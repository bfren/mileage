// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Domain.GetCar;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Repositories;
using Wrap.Caching;

namespace Mileage.Domain.GetPlace;

/// <summary>
/// Get a place
/// </summary>
internal sealed class GetPlaceHandler : QueryHandler<GetPlaceQuery, PlaceModel>
{
	private IWrapCache<PlaceId> Cache { get; init; }

	private IPlaceRepository Place { get; init; }

	private ILog<GetPlaceHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="cache"></param>
	/// <param name="place"></param>
	/// <param name="log"></param>
	public GetPlaceHandler(IWrapCache<PlaceId> cache, IPlaceRepository place, ILog<GetPlaceHandler> log) =>
		(Cache, Place, Log) = (cache, place, log);

	/// <summary>
	/// Get the specified place if it belongs to the user
	/// </summary>
	/// <param name="query"></param>
	public override async Task<Result<PlaceModel>> HandleAsync(GetPlaceQuery query)
	{
		if (query.PlaceId is null || query.PlaceId.Value == 0)
		{
			return R.Fail("Place ID cannot be null.")
				.Ctx(nameof(GetPlaceHandler), nameof(HandleAsync));
		}

		return await Cache
			.GetOrCreateAsync(
				key: query.PlaceId,
				valueFactory: () =>
				{
					Log.Vrb("Get Place: {Query}.", query);
					return Place
						.Fluent()
						.Where(x => x.Id, Compare.Equal, query.PlaceId)
						.Where(x => x.UserId, Compare.Equal, query.UserId)
						.QuerySingleAsync<PlaceModel>()
						.ToMaybeAsync(Log.Failure);
				}
			)
			.ToResultAsync(
				nameof(GetCarHandler), nameof(HandleAsync)
			)
			.IfNotAsync(
				fTest: x => x.UserId == query.UserId,
				fThen: _ => R.Fail("Place {PlaceId} does not belong to user {UserId}.", query.PlaceId.Value, query.UserId.Value)
					.Ctx(nameof(GetPlaceHandler), nameof(HandleAsync))
			);
	}
}
