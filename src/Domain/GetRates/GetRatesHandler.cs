// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Collections.Generic;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetRates;

/// <summary>
/// Get rates
/// </summary>
internal sealed class GetRatesHandler : QueryHandler<GetRatesQuery, IEnumerable<GetRatesModel>>
{
	private IRateRepository Rate { get; init; }

	private ILog<GetRatesHandler> Log { get; init; }

	/// <summary>
	/// Inject dependency
	/// </summary>
	/// <param name="rate"></param>
	/// <param name="log"></param>
	public GetRatesHandler(IRateRepository rate, ILog<GetRatesHandler> log) =>
		(Rate, Log) = (rate, log);

	/// <summary>
	/// Get rates for the specified user, sorted by amount
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<IEnumerable<GetRatesModel>>> HandleAsync(GetRatesQuery query)
	{
		if (query.UserId is null || query.UserId.Value == 0)
		{
			return F.None<IEnumerable<GetRatesModel>, Messages.UserIdIsNullMsg>().AsTask;
		}

		Log.Vrb("Get Rates for {User}.", query.UserId);
		return Rate
			.StartFluentQuery()
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.Sort(x => x.AmountPerMileGBP, SortOrder.Ascending)
			.QueryAsync<GetRatesModel>();
	}
}
