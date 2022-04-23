// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetLatestEndMiles;

/// <summary>
/// Get the latest 'end miles' of all journeys for the specified user - or 0 if not found
/// </summary>
internal sealed class GetLatestEndMilesHandler : QueryHandler<GetLatestEndMilesQuery, uint>
{
	private IJourneyRepository Journey { get; init; }

	private ILog<GetLatestEndMilesHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="log"></param>
	public GetLatestEndMilesHandler(IJourneyRepository journey, ILog<GetLatestEndMilesHandler> log) =>
		(Journey, Log) = (journey, log);

	/// <summary>
	/// Get the latest 'end miles' of all journeys for the user and car specified by <paramref name="query"/> -
	/// or 0 if not found / the most recent journey doesn't have end miles set
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<uint>> HandleAsync(GetLatestEndMilesQuery query)
	{
		if (query.CarId is null)
		{
			return F.Some(0u).AsTask;
		}

		Log.Vrb("Getting latest end miles for User {UserId} and Car {CarId}.", query.UserId.Value, query.CarId.Value);
		return Journey
			.StartFluentQuery()
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.Where(x => x.CarId, Compare.Equal, query.CarId)
			.Sort(x => x.StartMiles, SortOrder.Descending)
			.ExecuteAsync(x => x.EndMiles)
			.IfNullAsync(
				ifNull: () => 0u,
				ifSome: x => (uint)x!,
				F.DefaultHandler
			);
	}
}
