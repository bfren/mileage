// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Collections.Generic;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetJourneys;

/// <summary>
/// Get journeys
/// </summary>
internal sealed class GetJourneysHandler : QueryHandler<GetJourneysQuery, IEnumerable<JourneyModel>>
{
	private IJourneyRepository Journey { get; init; }

	private ILog<GetJourneysHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="log"></param>
	public GetJourneysHandler(IJourneyRepository journey, ILog<GetJourneysHandler> log) =>
		(Journey, Log) = (journey, log);

	/// <summary>
	/// Get journeys between dates specified in <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<IEnumerable<JourneyModel>>> HandleAsync(GetJourneysQuery query)
	{
		Log.Vrb("Getting Journeys for {User} between {From} and {To}.", query.UserId.Value, query.Start, query.End);
		return Journey
			.StartFluentQuery()
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.Where(x => x.Day, Compare.MoreThanOrEqual, query.Start)
			.Where(x => x.Day, Compare.LessThanOrEqual, query.End)
			.Sort(x => x.Day, SortOrder.Ascending)
			.Sort(x => x.StartMiles, SortOrder.Descending)
			.QueryAsync<JourneyModel>();
	}
}
