// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Collections.Generic;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetPlacesForUser;

public sealed class GetPlacesForUserHandler : QueryHandler<GetPlacesForUserQuery, IEnumerable<GetPlacesForUserModel>>
{
	private IPlaceRepository Place { get; init; }

	private ILog<GetPlacesForUserHandler> Log { get; init; }

	public GetPlacesForUserHandler(IPlaceRepository place, ILog<GetPlacesForUserHandler> log) =>
		(Place, Log) = (place, log);

	public override Task<Maybe<IEnumerable<GetPlacesForUserModel>>> HandleAsync(GetPlacesForUserQuery query)
	{
		Log.Vrb("Get places for {User}", query.UserId);
		return Place
			.StartFluentQuery()
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.Sort(x => x.Description, SortOrder.Ascending)
			.QueryAsync<GetPlacesForUserModel>();
	}
}
