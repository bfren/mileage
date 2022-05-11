// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetJourney;

/// <summary>
/// Get a journey
/// </summary>
internal sealed class GetJourneyHandler : QueryHandler<GetJourneyQuery, JourneyModel>
{
	private IJourneyRepository Journey { get; init; }

	private ILog<GetJourneyHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="log"></param>
	public GetJourneyHandler(IJourneyRepository journey, ILog<GetJourneyHandler> log) =>
		(Journey, Log) = (journey, log);

	/// <summary>
	/// Get the specified journey if it belongs to the user
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<JourneyModel>> HandleAsync(GetJourneyQuery query)
	{
		if (query.JourneyId is null || query.JourneyId.Value == 0)
		{
			return F.None<JourneyModel, Messages.JourneyIdIsNullMsg>().AsTask();
		}

		Log.Vrb("Get Journey: {Query}.", query);
		return Journey
			.StartFluentQuery()
			.Where(x => x.Id, Compare.Equal, query.JourneyId)
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.QuerySingleAsync<JourneyModel>();
	}
}
