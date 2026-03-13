// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Domain.CheckRateBelongsToUser;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveRate;

/// <summary>
/// Save a rate - create if it doesn't exist, or update if it does
/// </summary>
internal sealed class SaveRateHandler : QueryHandler<SaveRateQuery, RateId>
{
	private IDispatcher Dispatcher { get; init; }

	private IRateRepository Rate { get; init; }

	private ILog<SaveRateHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="dispatcher"></param>
	/// <param name="rate"></param>
	/// <param name="log"></param>
	public SaveRateHandler(IDispatcher dispatcher, IRateRepository rate, ILog<SaveRateHandler> log) =>
		(Dispatcher, Rate, Log) = (dispatcher, rate, log);

	/// <summary>
	/// Save the rate belonging to user specified in <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	public override async Task<Result<RateId>> HandleAsync(SaveRateQuery query)
	{
		Log.Vrb("Saving Rate {Query}.", query);

		// Ensure the rate belongs to the user
		if (query.Id is not null)
		{
			var rateBelongsToUser = await Dispatcher
					.SendAsync(new CheckRateBelongsToUserQuery(query.UserId, query.Id))
					.IsTrueAsync();

			if (!rateBelongsToUser)
			{
				return R.Fail("Rate {RateId} does not belong to user {UserId}.", query.Id.Value, query.UserId.Value)
					.Ctx(nameof(SaveRateHandler), nameof(HandleAsync));
			}
		}

		// Create or update rate
		return await Rate
			.Fluent()
			.Where(x => x.Id, Compare.Equal, query.Id)
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.QuerySingleAsync<RateEntity>()
			.MatchAsync(
				fOk: x => Dispatcher
					.SendAsync(new Internals.UpdateRateCommand(x.Id, query))
					.BindAsync(_ => R.Wrap(x.Id)),
				fFail: f => Dispatcher
					.SendAsync(new Internals.CreateRateQuery(query))
			);
	}
}
