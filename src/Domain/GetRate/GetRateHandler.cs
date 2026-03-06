// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Domain.GetCar;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Repositories;
using Wrap.Caching;

namespace Mileage.Domain.GetRate;

/// <summary>
/// Get a rate
/// </summary>
internal sealed class GetRateHandler : QueryHandler<GetRateQuery, RateModel>
{
	private IWrapCache<RateId> Cache { get; init; }

	private IRateRepository Rate { get; init; }

	private ILog<GetRateHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="cache"></param>
	/// <param name="rate"></param>
	/// <param name="log"></param>
	public GetRateHandler(IWrapCache<RateId> cache, IRateRepository rate, ILog<GetRateHandler> log) =>
		(Cache, Rate, Log) = (cache, rate, log);

	/// <summary>
	/// Get the specified rate if it belongs to the user
	/// </summary>
	/// <param name="query"></param>
	public override async Task<Result<RateModel>> HandleAsync(GetRateQuery query)
	{
		if (query.RateId is null || query.RateId.Value == 0)
		{
			return R.Fail("Rate ID cannot be null.")
				.Ctx(nameof(GetCarHandler), nameof(HandleAsync));
		}

		return await Cache
			.GetOrCreateAsync(
				key: query.RateId,
				valueFactory: () =>
				{
					Log.Vrb("Get Rate: {Query}.", query);
					return Rate
						.Fluent()
						.Where(x => x.Id, Compare.Equal, query.RateId)
						.Where(x => x.UserId, Compare.Equal, query.UserId)
						.QuerySingleAsync<RateModel>()
						.ToMaybeAsync(Log.Failure);
				}
			)
			.ToResultAsync(
				nameof(GetCarHandler), nameof(HandleAsync)
			)
			.IfNotAsync(
				fTest: x => x.UserId == query.UserId,
				fThen: _ => R.Fail("Rate {RateId} does not belong to user {UserId}.", query.RateId.Value, query.UserId.Value)
					.Ctx(nameof(GetRateHandler), nameof(HandleAsync))
			);
	}
}
