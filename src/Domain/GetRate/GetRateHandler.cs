// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using MaybeF.Caching;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetRate;

/// <summary>
/// Get a rate
/// </summary>
internal sealed class GetRateHandler : QueryHandler<GetRateQuery, RateModel>
{
	private IMaybeCache<RateId> Cache { get; init; }

	private IRateRepository Rate { get; init; }

	private ILog<GetRateHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="cache"></param>
	/// <param name="rate"></param>
	/// <param name="log"></param>
	public GetRateHandler(IMaybeCache<RateId> cache, IRateRepository rate, ILog<GetRateHandler> log) =>
		(Cache, Rate, Log) = (cache, rate, log);

	/// <summary>
	/// Get the specified rate if it belongs to the user
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<RateModel>> HandleAsync(GetRateQuery query)
	{
		if (query.RateId is null || query.RateId.Value == 0)
		{
			return F.None<RateModel, Messages.RateIdIsNullMsg>().AsTask();
		}

		return Cache
			.GetOrCreateAsync(
				key: query.RateId,
				valueFactory: () =>
				{
					Log.Vrb("Get Rate: {Query}.", query);
					return Rate
						.StartFluentQuery()
						.Where(x => x.Id, Compare.Equal, query.RateId)
						.Where(x => x.UserId, Compare.Equal, query.UserId)
						.QuerySingleAsync<RateModel>();
				}
			)
			.SwitchIfAsync(
				check: x => x.UserId == query.UserId,
				ifFalse: _ => F.None<RateModel, Messages.RateDoesNotBelongToUserMsg>()
			);
	}
}
