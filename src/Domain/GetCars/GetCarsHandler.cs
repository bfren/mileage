// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Collections.Generic;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetCars;

/// <summary>
/// Get cars
/// </summary>
internal sealed class GetCarsHandler : QueryHandler<GetCarsQuery, IEnumerable<GetCarsModel>>
{
	private ICarRepository Car { get; init; }

	private ILog<GetCarsHandler> Log { get; init; }

	/// <summary>
	/// Inject dependency
	/// </summary>
	/// <param name="car"></param>
	/// <param name="log"></param>
	public GetCarsHandler(ICarRepository car, ILog<GetCarsHandler> log) =>
		(Car, Log) = (car, log);

	/// <summary>
	/// Get cars for the specified user, sorted by description
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<IEnumerable<GetCarsModel>>> HandleAsync(GetCarsQuery query)
	{
		if (query.UserId is null || query.UserId.Value == 0)
		{
			return F.None<IEnumerable<GetCarsModel>, Messages.UserIdIsNullMsg>().AsTask;
		}

		Log.Vrb("Get Cars for {User}.", query.UserId);
		return Car
			.StartFluentQuery()
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.Sort(x => x.Description, SortOrder.Ascending)
			.QueryAsync<GetCarsModel>();
	}
}
