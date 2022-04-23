// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mileage.Domain.GetIncompleteJourneys;
using Mileage.Domain.GetRecentJourneys;
using Mileage.WebApp.Pages.Journeys;

namespace Mileage.WebApp.Pages;

[Authorize]
public sealed class IndexModel : PageModel
{
	private IDispatcher Dispatcher { get; init; }

	public IncompleteModel IncompleteJourneys { get; private set; } = new();

	public RecentModel RecentJourneys { get; private set; } = new();

	public IndexModel(IDispatcher dispatcher) =>
		Dispatcher = dispatcher;

	public async Task<IActionResult> OnGetAsync()
	{
		var query = from u in User.GetUserId()
					from incomplete in Dispatcher.DispatchAsync(new GetIncompleteJourneysQuery(u))
					from recent in Dispatcher.DispatchAsync(new GetRecentJourneysQuery(u))
					select new { incomplete, recent };

		await foreach (var j in query)
		{
			IncompleteJourneys = new() { Journeys = j.incomplete.ToList() };
			RecentJourneys = new() { Journeys = j.recent.ToList() };
			return Page();
		}

		return RedirectToPage("/auth/signout");
	}
}
