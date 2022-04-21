// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetPlace;

/// <summary>
/// Get a 'from' place
/// </summary>
internal sealed class GetPlaceHandler : QueryHandler<GetPlaceQuery, GetPlaceModel>
{
	private IPlaceRepository Place { get; init; }

	private ILog<GetPlaceHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="place"></param>
	/// <param name="log"></param>
	public GetPlaceHandler(IPlaceRepository place, ILog<GetPlaceHandler> log) =>
		(Place, Log) = (place, log);

	/// <summary>
	/// Get the specified place if it belongs to the user
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<GetPlaceModel>> HandleAsync(GetPlaceQuery query)
	{
		if (query.PlaceId is null || query.PlaceId.Value == 0)
		{
			return F.None<GetPlaceModel, Messages.PlaceIdIsNullMsg>().AsTask;
		}

		Log.Vrb("Get Place: {Query}.", query);
		return Place
			.StartFluentQuery()
			.Where(x => x.Id, Compare.Equal, query.PlaceId)
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.QuerySingleAsync<GetPlaceModel>();
	}
}
