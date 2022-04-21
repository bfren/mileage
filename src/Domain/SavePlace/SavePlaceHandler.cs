// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Domain.CheckPlacesBelongToUser;
using Mileage.Domain.SavePlace.Messages;
using Mileage.Persistence.Common.StrongIds;
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
	public override async Task<Maybe<PlaceId>> HandleAsync(SavePlaceQuery query)
	{
		// Ensure the place belongs to the user
		if (query.Id is not null)
		{
			var placeBelongsToUser = await Dispatcher
					.DispatchAsync(new CheckPlacesBelongToUserQuery(query.UserId, query.Id))
					.IsTrueAsync();

			if (!placeBelongsToUser)
			{
				return F.None<PlaceId>(new PlaceDoesNotBelongToUserMsg(query.UserId, query.Id));
			}
		}

		// Create or update place
		return await Place
			.StartFluentQuery()
			.Where(x => x.Id, Compare.Equal, query.Id)
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.QuerySingleAsync<PlaceEntity>()
			.SwitchAsync(
				some: x => Dispatcher
					.DispatchAsync(new Internals.UpdatePlaceCommand(x.Id, query.Version, query.Description, query.Postcode))
					.BindAsync(_ => F.Some(x.Id)),
				none: () => Dispatcher
					.DispatchAsync(new Internals.CreatePlaceQuery(query.UserId, query.Description, query.Postcode))
			);
	}
}
