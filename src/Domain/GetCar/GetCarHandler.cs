// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Repositories;
using Wrap.Caching;

namespace Mileage.Domain.GetCar;

/// <summary>
/// Get a car
/// </summary>
internal sealed class GetCarHandler : QueryHandler<GetCarQuery, CarModel>
{
	private IWrapCache<CarId> Cache { get; init; }

	private ICarRepository Car { get; init; }

	private ILog<GetCarHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="cache"></param>
	/// <param name="car"></param>
	/// <param name="log"></param>
	public GetCarHandler(IWrapCache<CarId> cache, ICarRepository car, ILog<GetCarHandler> log) =>
		(Cache, Car, Log) = (cache, car, log);

	/// <summary>
	/// Get the specified car if it belongs to the user
	/// </summary>
	/// <param name="query"></param>
	public override async Task<Result<CarModel>> HandleAsync(GetCarQuery query)
	{
		if (query.CarId is null || query.CarId.Value == 0)
		{
			return R.Fail("Car ID cannot be null.")
				.Ctx(nameof(GetCarHandler), nameof(HandleAsync));
		}

		return await Cache
			.GetOrCreateAsync(
				key: query.CarId,
				valueFactory: () =>
				{
					Log.Vrb("Get Car: {Query}.", query);
					return Car.Fluent()
						.Where(x => x.Id, Compare.Equal, query.CarId)
						.Where(x => x.UserId, Compare.Equal, query.UserId)
						.QuerySingleAsync<CarModel>()
						.ToMaybeAsync(Log.Failure);
				}
			)
			.ToResultAsync(
				nameof(GetCarHandler), nameof(HandleAsync)
			)
			.IfNotAsync(
				fTest: x => x.UserId == query.UserId,
				fThen: _ => R.Fail("Car {CarId} does not belong to user {UserId}.", query.CarId.Value, query.UserId.Value)
					.Ctx(nameof(GetCarHandler), nameof(HandleAsync))
			);
	}
}
