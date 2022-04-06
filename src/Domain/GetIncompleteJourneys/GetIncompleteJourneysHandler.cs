// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Collections.Generic;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetIncompleteJourneys;

/// <summary>
/// Get incomplete journeys
/// </summary>
internal sealed class GetIncompleteJourneysHandler : QueryHandler<GetIncompleteJourneysQuery, IEnumerable<IncompleteJourneyModel>>
{
	private IJourneyRepository Journey { get; init; }

	private ILog<GetIncompleteJourneysHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="log"></param>
	public GetIncompleteJourneysHandler(IJourneyRepository journey, ILog<GetIncompleteJourneysHandler> log) =>
		(Journey, Log) = (journey, log);

	/// <summary>
	/// Get journeys for user in <paramref name="query"/> where end miles is null
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<IEnumerable<IncompleteJourneyModel>>> HandleAsync(GetIncompleteJourneysQuery query)
	{
		Log.Vrb("Getting incomplete journeys for user {UserId}.", query.UserId.Value);
		return Journey
			.StartFluentQuery()
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.Where(x => x.EndMiles, Compare.Is, null)
			.Sort(x => x.StartMiles, SortOrder.Descending)
			.QueryAsync<IncompleteJourneyModel>();
	}
}
