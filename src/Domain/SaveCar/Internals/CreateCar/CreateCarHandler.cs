// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveCar.Internals.CreateCar;

/// <summary>
/// Create a new car entity
/// </summary>
public sealed class CreateCarHandler : QueryHandler<CreateCarQuery, CarId>
{
	private ICarRepository Car { get; init; }

	private ILog<CreateCarHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="car"></param>
	/// <param name="log"></param>
	public CreateCarHandler(ICarRepository car, ILog<CreateCarHandler> log) =>
		(Car, Log) = (car, log);

	/// <summary>
	/// Create a new car from <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<CarId>> HandleAsync(CreateCarQuery query)
	{
		Log.Vrb("Create Car: {Query}", query);
		return Car
			.CreateAsync(new()
			{
				UserId = query.UserId,
				Description = query.Description
			});
	}
}
