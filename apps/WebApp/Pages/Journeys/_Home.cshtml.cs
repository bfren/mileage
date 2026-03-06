// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetIncompleteJourneys;
using Mileage.Domain.GetRecentJourneys;

namespace Mileage.WebApp.Pages.Journeys;

public sealed record class HomeModel
{
	public IncompleteModel IncompleteJourneys { get; set; } = new();

	public RecentModel RecentJourneys { get; set; } = new();
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetHomeAsync() =>
		CreateHomeModel(
			User.GetUserId(), Dispatcher, Log
		)
		.MatchAsync(
			fOk: x => Partial("_Home", x),
			fFail: r => Partial("_Error", r)
		);

	internal static Task<Result<HomeModel>> CreateHomeModel(Maybe<AuthUserId> user, IDispatcher dispatcher, ILog log)
	{
		var query = from u in user
					from incomplete in dispatcher.SendAsync(new GetIncompleteJourneysQuery(u))
					from recent in dispatcher.SendAsync(new GetRecentJourneysQuery(u))
					select new { incomplete, recent };

		return query
			.AuditAsync(fFail: log.Failure)
			.MapAsync(
				x => new HomeModel
				{
					IncompleteJourneys = new() { Journeys = [.. x.incomplete] },
					RecentJourneys = new() { Journeys = [.. x.recent] }
				}
			);
	}
}
