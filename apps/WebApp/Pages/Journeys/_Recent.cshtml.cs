// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetRecentJourneys;

namespace Mileage.WebApp.Pages.Journeys;

public sealed record class RecentModel
{
	public List<RecentJourneyModel> Journeys { get; init; } = [];
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetRecentAsync()
	{
		var query = from u in User.GetUserId()
					from j in Dispatcher.DispatchAsync(new GetRecentJourneysQuery(u))
					select j;

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("_Recent", new RecentModel { Journeys = x.ToList() }),
				none: _ => Partial("_Recent", new RecentModel())
			);
	}
}
