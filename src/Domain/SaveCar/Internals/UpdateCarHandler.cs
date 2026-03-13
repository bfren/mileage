// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Repositories;
using Wrap.Caching;

namespace Mileage.Domain.SaveCar.Internals;

/// <summary>
/// Update an existing car entity
/// </summary>
internal sealed class UpdateCarHandler : CommandHandler<UpdateCarCommand>
{
	private IWrapCache<CarId> Cache { get; init; }

	private ICarRepository Car { get; init; }

	private ILog<UpdateCarHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="cache"></param>
	/// <param name="car"></param>
	/// <param name="log"></param>
	public UpdateCarHandler(IWrapCache<CarId> cache, ICarRepository car, ILog<UpdateCarHandler> log) =>
		(Cache, Car, Log) = (cache, car, log);

	/// <summary>
	/// Update a car from <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Result<bool>> HandleAsync(UpdateCarCommand command)
	{
		Log.Vrb("Update Car: {Command}", command);
		return Car
			.UpdateAsync(command)
			.IfOkAsync(x => { if (x) { Cache.RemoveValue(command.Id); } });
	}
}
