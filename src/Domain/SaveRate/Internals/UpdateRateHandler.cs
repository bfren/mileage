// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using MaybeF.Caching;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveRate.Internals;

/// <summary>
/// Update an existing rate entity
/// </summary>
internal sealed class UpdateRateHandler : CommandHandler<UpdateRateCommand>
{
	private IMaybeCache<RateId> Cache { get; init; }

	private IRateRepository Rate { get; init; }

	private ILog<UpdateRateHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="cache"></param>
	/// <param name="rate"></param>
	/// <param name="log"></param>
	public UpdateRateHandler(IMaybeCache<RateId> cache, IRateRepository rate, ILog<UpdateRateHandler> log) =>
		(Cache, Rate, Log) = (cache, rate, log);

	/// <summary>
	/// Update a rate from <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(UpdateRateCommand command)
	{
		Log.Vrb("Update Rate: {Command}", command);
		return Rate
			.UpdateAsync(command)
			.IfSomeAsync(x => { if (x) { Cache.RemoveValue(command.Id); } });
	}
}
