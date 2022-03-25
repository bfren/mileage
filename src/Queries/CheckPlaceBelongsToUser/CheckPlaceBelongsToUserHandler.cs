// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Queries.CheckPlaceBelongsToUser;

/// <summary>
/// Check a place belongs to a user
/// </summary>
public sealed class CheckPlaceBelongsToUserHandler : QueryHandler<CheckPlaceBelongsToUserQuery, bool>
{
	private IPlaceRepository Place { get; init; }

	private ILog<CheckPlaceBelongsToUserQuery> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="place"></param>
	/// <param name="log"></param>
	public CheckPlaceBelongsToUserHandler(IPlaceRepository place, ILog<CheckPlaceBelongsToUserQuery> log) =>
		(Place, Log) = (place, log);

	/// <summary>
	/// Returns true if the place belongs to the user defined by <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	/// <param name="cancellationToken"></param>
	public override Task<Maybe<bool>> HandleAsync(CheckPlaceBelongsToUserQuery query, CancellationToken cancellationToken)
	{
		Log.Vrb("Checking place {PlaceId} belongs to user {UserId}.", query.PlaceId.Value, query.UserId.Value);
		return Place
			.StartFluentQuery()
			.Where(
				c => c.Id, Compare.Equal, query.PlaceId
			)
			.Where(
				c => c.UserId, Compare.Equal, query.UserId
			)
			.QuerySingleAsync<CarEntity>()
			.SwitchAsync(
				some: _ => F.True,
				none: _ => F.False
			);
	}
}