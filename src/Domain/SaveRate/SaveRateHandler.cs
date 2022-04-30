// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Domain.CheckRateBelongsToUser;
using Mileage.Domain.SaveRate.Messages;
using Mileage.Persistence.Common.StrongIds;
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
	public override async Task<Maybe<RateId>> HandleAsync(SaveRateQuery query)
	{
		Log.Vrb("Saving Rate {Query}.", query);

		// Ensure the rate belongs to the user
		if (query.Id is not null)
		{
			var rateBelongsToUser = await Dispatcher
					.DispatchAsync(new CheckRateBelongsToUserQuery(query.UserId, query.Id))
					.IsTrueAsync();

			if (!rateBelongsToUser)
			{
				return F.None<RateId>(new RateDoesNotBelongToUserMsg(query.UserId, query.Id));
			}
		}

		// Create or update rate
		return await Rate
			.StartFluentQuery()
			.Where(x => x.Id, Compare.Equal, query.Id)
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.QuerySingleAsync<RateEntity>()
			.SwitchAsync(
				some: x => Dispatcher
					.DispatchAsync(new Internals.UpdateRateCommand(x.Id, query))
					.BindAsync(_ => F.Some(x.Id)),
				none: () => Dispatcher
					.DispatchAsync(new Internals.CreateRateQuery(query))
			);
	}
}
