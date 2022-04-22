// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Apps.Web.Constants;
using Jeebs.Cqrs;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mileage.Domain.GetIncompleteJourneys;
using Mileage.WebApp.Pages.Journeys;

namespace Mileage.WebApp.Pages;

#if DEBUG
[ResponseCache(CacheProfileName = CacheProfiles.None)]
#else
[ResponseCache(CacheProfileName = CacheProfiles.Default)]
#endif
[Authorize]
public sealed class IndexModel : PageModel
{
	private IDispatcher Dispatcher { get; init; }

	public IncompleteModel IncompleteJourneys { get; private set; } = new();

	public IndexModel(IDispatcher dispatcher) =>
		Dispatcher = dispatcher;

	public async Task<IActionResult> OnGetAsync()
	{
		var query = from u in User.GetUserId()
					from j in Dispatcher.DispatchAsync(new GetIncompleteJourneysQuery(u))
					select j;

		await foreach (var journeys in query)
		{
			IncompleteJourneys = new() { Journeys = journeys.ToList() };
			return Page();
		}

		return RedirectToPage("/auth/signout");
	}
}
