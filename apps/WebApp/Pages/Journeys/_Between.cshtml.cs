// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetJourneys;

namespace Mileage.WebApp.Pages.Journeys;

public sealed record class BetweenModel
{
	public DateTime? Start { get; init; }

	public DateTime? End { get; init; }

	public List<JourneyModel> Journeys { get; init; } = [];
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetBetweenAsync(DateTime start, DateTime end) =>
		GetJourneysBetweenAsync(start, end)
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync(
				fOk: x => Partial("_Between", new BetweenModel { Start = start, End = end, Journeys = [.. x] }),
				fFail: _ => Partial("_Between", new BetweenModel())
			);

	public Task<Result<IEnumerable<JourneyModel>>> GetJourneysBetweenAsync(DateTime start, DateTime end) =>
		from u in User.GetUserId()
		from d in Dispatcher.SendAsync(new GetJourneysQuery(u, start, end))
		select d;
}
