// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mileage.Domain.GetJourneys;

namespace Mileage.WebApp.Pages.Reports;

public sealed class JourneysModel : PageModel
{
	private IDispatcher Dispatcher { get; init; }

	private ILog<JourneysModel> Log { get; init; }

	public DateTime From { get; set; }

	public DateTime To { get; set; }

	public List<JourneyModel> Journeys { get; set; } = new();

	public JourneysModel(IDispatcher dispatcher, ILog<JourneysModel> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync(DateTime start, DateTime end)
	{
		From = start;
		To = end;

		var query = from u in User.GetUserId()
					from d in Dispatcher.DispatchAsync(new GetJourneysQuery(u, start, end))
					select d;

		await foreach (var item in query)
		{
			Journeys = item.ToList();
		}

		return Page();
	}
}
