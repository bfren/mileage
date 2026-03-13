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
	public override async Task<Result<JourneyModel>> HandleAsync(GetJourneyQuery query)
	{
		if (query.JourneyId is null || query.JourneyId.Value == 0)
		{
			return R.Fail("Journey ID cannot be null.")
				.Ctx(nameof(GetJourneyHandler), nameof(HandleAsync));
		}

		Log.Vrb("Get Journey: {Query}.", query);
		return await Journey
			.Fluent()
			.Where(x => x.Id, Compare.Equal, query.JourneyId)
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.QuerySingleAsync<JourneyModel>();
	}
}
