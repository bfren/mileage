// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mileage.Domain.GetRates;
using Mileage.Domain.SaveRate;

namespace Mileage.WebApp.Pages.Settings.Rates;

[Authorize]
[ValidateAntiForgeryToken]
public sealed partial class IndexModel : PageModel
{
	private IDispatcher Dispatcher { get; }

	private ILog<IndexModel> Log { get; }

	public List<GetRatesModel> Rates { get; set; } = new();

	public IndexModel(IDispatcher dispatcher, ILog<IndexModel> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync()
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.DispatchAsync(new GetRatesQuery(u))
					select r;

		await foreach (var rates in query)
		{
			Rates = rates.ToList();
		}

		return Page();
	}

	public Task<IActionResult> OnPostAsync(SaveRateQuery rate)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.DispatchAsync(rate with { UserId = u })
					select r;

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: _ => OnGetAsync(),
				none: r => Result.Error(r)
			);
	}
}
