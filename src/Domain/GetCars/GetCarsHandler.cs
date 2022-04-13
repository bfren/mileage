// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Collections.Generic;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetCars;

internal sealed class GetCarsHandler : QueryHandler<GetCarsQuery, IEnumerable<GetCarsModel>>
{
	private ICarRepository Car { get; init; }

	private ILog<GetCarsHandler> Log { get; init; }

	public GetCarsHandler(ICarRepository car, ILog<GetCarsHandler> log) =>
		(Car, Log) = (car, log);

	public override Task<Maybe<IEnumerable<GetCarsModel>>> HandleAsync(GetCarsQuery query)
	{
		Log.Vrb("Get cars for {User}", query.UserId);
		return Car
			.StartFluentQuery()
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.Sort(x => x.Description, SortOrder.Ascending)
			.QueryAsync<GetCarsModel>();
	}
}
