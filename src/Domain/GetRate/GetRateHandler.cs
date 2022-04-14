// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetRate;

/// <summary>
/// Get a car
/// </summary>
internal sealed class GetRateHandler : QueryHandler<GetRateQuery, GetRateModel>
{
	private IRateRepository Rate { get; init; }

	private ILog<GetRateHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="car"></param>
	/// <param name="log"></param>
	public GetRateHandler(IRateRepository car, ILog<GetRateHandler> log) =>
		(Rate, Log) = (car, log);

	/// <summary>
	/// Get the specified car if it belongs to the user
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<GetRateModel>> HandleAsync(GetRateQuery query)
	{
		if (query.RateId is null || query.RateId.Value == 0)
		{
			return F.None<GetRateModel, Messages.RateIdIsNullMsg>().AsTask;
		}

		Log.Vrb("Get Rate: {Query}.", query);
		return Rate
			.StartFluentQuery()
			.Where(x => x.Id, Compare.Equal, query.RateId)
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.QuerySingleAsync<GetRateModel>();
	}
}
