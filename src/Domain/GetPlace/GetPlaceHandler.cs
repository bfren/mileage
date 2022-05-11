// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using MaybeF.Caching;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetPlace;

/// <summary>
/// Get a place
/// </summary>
internal sealed class GetPlaceHandler : QueryHandler<GetPlaceQuery, PlaceModel>
{
	private IMaybeCache<PlaceId> Cache { get; init; }

	private IPlaceRepository Place { get; init; }

	private ILog<GetPlaceHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="cache"></param>
	/// <param name="place"></param>
	/// <param name="log"></param>
	public GetPlaceHandler(IMaybeCache<PlaceId> cache, IPlaceRepository place, ILog<GetPlaceHandler> log) =>
		(Cache, Place, Log) = (cache, place, log);

	/// <summary>
	/// Get the specified place if it belongs to the user
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<PlaceModel>> HandleAsync(GetPlaceQuery query)
	{
		if (query.PlaceId is null || query.PlaceId.Value == 0)
		{
			return F.None<PlaceModel, Messages.PlaceIdIsNullMsg>().AsTask();
		}

		return Cache
			.GetOrCreateAsync(
				key: query.PlaceId,
				valueFactory: () =>
				{
					Log.Vrb("Get Place: {Query}.", query);
					return Place
						.StartFluentQuery()
						.Where(x => x.Id, Compare.Equal, query.PlaceId)
						.Where(x => x.UserId, Compare.Equal, query.UserId)
						.QuerySingleAsync<PlaceModel>();
				}
			)
			.SwitchIfAsync(
				check: x => x.UserId == query.UserId,
				ifFalse: _ => F.None<PlaceModel, Messages.PlaceDoesNotBelongToUserMsg>()
			);
	}
}
