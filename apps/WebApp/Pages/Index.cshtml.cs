// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Apps.Web.Constants;
using Jeebs.Cqrs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mileage.Domain.GetIncompleteJourneys;

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

	public IList<IncompleteJourneyModel> IncompleteJourneys { get; private set; } = new List<IncompleteJourneyModel>();

	public IndexModel(IDispatcher dispatcher) =>
		Dispatcher = dispatcher;

	public async Task OnGetAsync()
	{
		IncompleteJourneys = await Dispatcher.DispatchAsync(
			new GetIncompleteJourneysQuery(new() { Value = 68 })
		).SwitchAsync(
			some: x => x.ToList(),
			none: _ => new List<IncompleteJourneyModel>()
		);

		return;
	}
}
