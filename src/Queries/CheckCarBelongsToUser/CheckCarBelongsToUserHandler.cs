// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Queries.CheckCarBelongsToUser;

/// <summary>
/// Check a car belongs to a user
/// </summary>
public sealed class CheckCarBelongsToUserHandler : QueryHandler<CheckCarBelongsToUserQuery, bool>
{
	private ICarRepository Car { get; init; }

	private ILog<CheckCarBelongsToUserHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="car"></param>
	/// <param name="log"></param>
	public CheckCarBelongsToUserHandler(ICarRepository car, ILog<CheckCarBelongsToUserHandler> log) =>
		(Car, Log) = (car, log);

	/// <summary>
	/// Returns true if the car belongs to the user defined by <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	/// <param name="cancellationToken"></param>
	public override Task<Maybe<bool>> HandleAsync(CheckCarBelongsToUserQuery query, CancellationToken cancellationToken)
	{
		Log.Vrb("Checking car {CarId} belongs to user {UserId}.", query.CarId.Value, query.UserId.Value);
		return Car
			.StartFluentQuery()
			.Where(
				c => c.Id, Compare.Equal, query.CarId
			)
			.Where(
				c => c.UserId, Compare.Equal, query.UserId
			)
			.QuerySingleAsync<CarEntity>()
			.AuditAsync(
				none: Log.Msg
			)
			.SwitchAsync(
				some: _ => F.True,
				none: _ => F.False
			);
	}
}
