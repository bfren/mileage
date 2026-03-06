// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Domain.CheckCarBelongsToUser;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveCar;

/// <summary>
/// Save a car - create if it doesn't exist, or update if it does
/// </summary>
internal sealed class SaveCarHandler : QueryHandler<SaveCarQuery, CarId>
{
	private IDispatcher Dispatcher { get; init; }

	private ICarRepository Car { get; init; }

	private ILog<SaveCarHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="dispatcher"></param>
	/// <param name="car"></param>
	/// <param name="log"></param>
	public SaveCarHandler(IDispatcher dispatcher, ICarRepository car, ILog<SaveCarHandler> log) =>
		(Dispatcher, Car, Log) = (dispatcher, car, log);

	/// <summary>
	/// Save the car belonging to user specified in <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	public override async Task<Result<CarId>> HandleAsync(SaveCarQuery query)
	{
		Log.Vrb("Saving Car {Query}.", query);

		// Ensure the car belongs to the user
		if (query.Id?.Value > 0)
		{
			var carBelongsToUser = await Dispatcher
					.SendAsync(new CheckCarBelongsToUserQuery(query.UserId, query.Id))
					.IsTrueAsync();

			if (!carBelongsToUser)
			{
				return R.Fail("Car {CarId} does not belong to user {UserId}.", query.Id.Value, query.UserId.Value)
					.Ctx(nameof(SaveCarHandler), nameof(HandleAsync));
			}
		}

		// Create or update car
		return await Car
			.Fluent()
			.Where(x => x.Id, Compare.Equal, query.Id)
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.QuerySingleAsync<CarEntity>()
			.MatchAsync(
				fOk: x => Dispatcher
					.SendAsync(new Internals.UpdateCarCommand(x.Id, query))
					.BindAsync(_ => R.Wrap(x.Id)),
				fFail: f => Dispatcher
					.SendAsync(new Internals.CreateCarQuery(query))
			);
	}
}
