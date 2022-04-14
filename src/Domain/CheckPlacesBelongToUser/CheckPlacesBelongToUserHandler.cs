// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Linq;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.CheckPlacesBelongToUser;

/// <summary>
/// Check a place belongs to a user
/// </summary>
internal sealed class CheckPlacesBelongToUserHandler : QueryHandler<CheckPlacesBelongToUserQuery, bool>
{
	private IPlaceRepository Place { get; init; }

	private ILog<CheckPlacesBelongToUserHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="place"></param>
	/// <param name="log"></param>
	public CheckPlacesBelongToUserHandler(IPlaceRepository place, ILog<CheckPlacesBelongToUserHandler> log) =>
		(Place, Log) = (place, log);

	/// <summary>
	/// Returns true if the place belongs to the user defined by <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<bool>> HandleAsync(CheckPlacesBelongToUserQuery query)
	{
		if (query.PlaceIds.Length == 0)
		{
			return F.None<bool, Messages.PlaceIdsIsNullMsg>().AsTask;
		}

		Log.Vrb("Checking places {PlaceIds} belong to user {UserId}.", query.PlaceIds.Select(p => p.Value), query.UserId.Value);
		return Place
			.StartFluentQuery()
			.WhereIn(c => c.Id, query.PlaceIds)
			.Where(c => c.UserId, Compare.Equal, query.UserId)
			.QueryAsync<PlaceEntity>()
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => x.Count() switch
				{
					int y when y == query.PlaceIds.Length =>
						F.True,

					_ =>
						F.False
				},
				none: _ => F.False
			);
	}
}
