// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Repositories;
using Wrap.Caching;

namespace Mileage.Domain.SaveRate.Internals;

/// <summary>
/// Update an existing rate entity
/// </summary>
internal sealed class UpdateRateHandler : CommandHandler<UpdateRateCommand>
{
	private IWrapCache<RateId> Cache { get; init; }

	private IRateRepository Rate { get; init; }

	private ILog<UpdateRateHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="cache"></param>
	/// <param name="rate"></param>
	/// <param name="log"></param>
	public UpdateRateHandler(IWrapCache<RateId> cache, IRateRepository rate, ILog<UpdateRateHandler> log) =>
		(Cache, Rate, Log) = (cache, rate, log);

	/// <summary>
	/// Update a rate from <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Result<bool>> HandleAsync(UpdateRateCommand command)
	{
		Log.Vrb("Update Rate: {Command}", command);
		return Rate
			.UpdateAsync(command)
			.IfOkAsync(x => { if (x) { Cache.RemoveValue(command.Id); } });
	}
}
