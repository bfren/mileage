// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveCar.Internals;

/// <summary>
/// Update an existing car entity
/// </summary>
internal sealed class UpdateCarHandler : CommandHandler<UpdateCarCommand>
{
	private ICarRepository Car { get; init; }

	private ILog<UpdateCarHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="car"></param>
	/// <param name="log"></param>
	public UpdateCarHandler(ICarRepository car, ILog<UpdateCarHandler> log) =>
		(Car, Log) = (car, log);

	/// <summary>
	/// Update a car from <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(UpdateCarCommand command)
	{
		Log.Vrb("Update Car: {Command}", command);
		return Car
			.UpdateAsync(new CarEntity
			{
				Id = command.CarId,
				Version = command.Version,
				Description = command.Description
			});
	}
}
