// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Collections.Generic;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetCarsForUser;

public sealed class GetCarsForUserHandler : QueryHandler<GetCarsForUserQuery, IEnumerable<GetCarsForUserModel>>
{
	private ICarRepository Car { get; init; }

	private ILog<GetCarsForUserHandler> Log { get; init; }

	public GetCarsForUserHandler(ICarRepository car, ILog<GetCarsForUserHandler> log) =>
		(Car, Log) = (car, log);

	public override Task<Maybe<IEnumerable<GetCarsForUserModel>>> HandleAsync(GetCarsForUserQuery query)
	{
		Log.Vrb("Get cars for {User}", query.UserId);
		return Car
			.StartFluentQuery()
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.Sort(x => x.Description, SortOrder.Ascending)
			.QueryAsync<GetCarsForUserModel>();
	}
}
