// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Domain.CheckPlacesBelongToUser;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SavePlace;

/// <summary>
/// Save a place - create if it doesn't exist, or update if it does
/// </summary>
internal sealed class SavePlaceHandler : QueryHandler<SavePlaceQuery, PlaceId>
{
	private IDispatcher Dispatcher { get; init; }

	private IPlaceRepository Place { get; init; }

	private ILog<SavePlaceHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="dispatcher"></param>
	/// <param name="place"></param>
	/// <param name="log"></param>
	public SavePlaceHandler(IDispatcher dispatcher, IPlaceRepository place, ILog<SavePlaceHandler> log) =>
		(Dispatcher, Place, Log) = (dispatcher, place, log);

	/// <summary>
	/// Save the place belonging to user specified in <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	public override async Task<Result<PlaceId>> HandleAsync(SavePlaceQuery query)
	{
		Log.Vrb("Saving Place {Query}.", query);

		// Ensure the place belongs to the user
		if (query.Id is not null)
		{
			var placeBelongsToUser = await Dispatcher
				.SendAsync(new CheckPlacesBelongToUserQuery(query.UserId, query.Id))
				.IsTrueAsync();

			if (!placeBelongsToUser)
			{
				return R.Fail("Place {PlaceId} does not belong to user {UserId}.", query.Id.Value, query.UserId.Value)
					.Ctx(nameof(SavePlaceHandler), nameof(HandleAsync));
			}
		}

		// Create or update place
		return await Place
			.Fluent()
			.Where(x => x.Id, Compare.Equal, query.Id)
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.QuerySingleAsync<PlaceEntity>()
			.MatchAsync(
				fOk: x => Dispatcher
					.SendAsync(new Internals.UpdatePlaceCommand(x.Id, query))
					.BindAsync(_ => R.Wrap(x.Id)),
				fFail: f => Dispatcher
					.SendAsync(new Internals.CreatePlaceQuery(query))
			);
	}
}
