// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mileage.Domain.GetPlaces;
using Mileage.Domain.SavePlace;

namespace Mileage.WebApp.Pages.Settings.Places;

[Authorize]
[ValidateAntiForgeryToken]
public sealed partial class IndexModel : PageModel
{
	private IDispatcher Dispatcher { get; }

	private ILog<IndexModel> Log { get; }

	public List<PlacesModel> Places { get; set; } = [];

	public IndexModel(IDispatcher dispatcher, ILog<IndexModel> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync()
	{
		var query = from u in User.GetUserId()
					from p in Dispatcher.SendAsync(new GetPlacesQuery(u, true))
					select p;

		foreach (var places in await query.Unsafe())
		{
			Places = [.. places];
		}

		return Page();
	}

	public Task<IActionResult> OnPostAsync(SavePlaceQuery place)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.SendAsync(place with { UserId = u })
					select r;

		return query
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync(
				fOk: _ => OnGetAsync(),
				fFail: Op.Error
			);
	}
}
