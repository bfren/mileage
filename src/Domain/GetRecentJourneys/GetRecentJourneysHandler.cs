// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Collections.Generic;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetRecentJourneys;

/// <summary>
/// Get recent journeys
/// </summary>
internal sealed class GetRecentJourneysHandler : QueryHandler<GetRecentJourneysQuery, IEnumerable<RecentJourneyModel>>
{
	private IJourneyRepository Journey { get; init; }

	private ILog<GetRecentJourneysHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="log"></param>
	public GetRecentJourneysHandler(IJourneyRepository journey, ILog<GetRecentJourneysHandler> log) =>
		(Journey, Log) = (journey, log);

	/// <summary>
	/// Get recent journeys for user in <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<IEnumerable<RecentJourneyModel>>> HandleAsync(GetRecentJourneysQuery query)
	{
		Log.Vrb("Getting recent journeys for user {UserId}.", query.UserId.Value);
		return Journey
			.StartFluentQuery()
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.Where(x => x.EndMiles, Compare.MoreThan, 0)
			.Sort(x => x.Day, SortOrder.Descending)
			.Sort(x => x.CarId, SortOrder.Ascending)
			.Sort(x => x.StartMiles, SortOrder.Descending)
			.Maximum(5)
			.QueryAsync<RecentJourneyModel>();
	}
}
