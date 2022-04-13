// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Collections.Generic;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetPlaces;

internal sealed class GetPlacesHandler : QueryHandler<GetPlacesQuery, IEnumerable<GetPlacesModel>>
{
	private IPlaceRepository Place { get; init; }

	private ILog<GetPlacesHandler> Log { get; init; }

	public GetPlacesHandler(IPlaceRepository place, ILog<GetPlacesHandler> log) =>
		(Place, Log) = (place, log);

	public override Task<Maybe<IEnumerable<GetPlacesModel>>> HandleAsync(GetPlacesQuery query)
	{
		Log.Vrb("Get places for {User}", query.UserId);
		return Place
			.StartFluentQuery()
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.Sort(x => x.Description, SortOrder.Ascending)
			.QueryAsync<GetPlacesModel>();
	}
}
