// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using MaybeF.Caching;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetCar;

/// <summary>
/// Get a car
/// </summary>
internal sealed class GetCarHandler : QueryHandler<GetCarQuery, CarModel>
{
	private IMaybeCache<CarId> Cache { get; init; }

	private ICarRepository Car { get; init; }

	private ILog<GetCarHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="cache"></param>
	/// <param name="car"></param>
	/// <param name="log"></param>
	public GetCarHandler(IMaybeCache<CarId> cache, ICarRepository car, ILog<GetCarHandler> log) =>
		(Cache, Car, Log) = (cache, car, log);

	/// <summary>
	/// Get the specified car if it belongs to the user
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<CarModel>> HandleAsync(GetCarQuery query)
	{
		if (query.CarId is null || query.CarId.Value == 0)
		{
			return F.None<CarModel, Messages.CarIdIsNullMsg>().AsTask;
		}

		return Cache
			.GetOrCreateAsync(
				key: query.CarId,
				valueFactory: () =>
				{
					Log.Vrb("Get Car: {Query}.", query);
					return Car.StartFluentQuery()
						.Where(x => x.Id, Compare.Equal, query.CarId)
						.Where(x => x.UserId, Compare.Equal, query.UserId)
						.QuerySingleAsync<CarModel>();
				}
			)
			.SwitchIfAsync(
				check: x => x.UserId == query.UserId,
				ifFalse: _ => F.None<CarModel, Messages.CarDoesNotBelongToUserMsg>()
			);
	}
}
