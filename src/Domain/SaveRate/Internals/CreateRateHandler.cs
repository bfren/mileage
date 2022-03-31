// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveRate.Internals;

/// <summary>
/// Create a new rate entity
/// </summary>
internal sealed class CreateRateHandler : QueryHandler<CreateRateQuery, RateId>
{
	private IRateRepository Rate { get; init; }

	private ILog<CreateRateHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="rate"></param>
	/// <param name="log"></param>
	public CreateRateHandler(IRateRepository rate, ILog<CreateRateHandler> log) =>
		(Rate, Log) = (rate, log);

	/// <summary>
	/// Create a new rate from <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<RateId>> HandleAsync(CreateRateQuery query)
	{
		Log.Vrb("Create Rate: {Query}", query);
		return Rate
			.CreateAsync(new()
			{
				UserId = query.UserId,
				AmountPerMileGBP = query.AmountPerMileGBP
			});
	}
}
