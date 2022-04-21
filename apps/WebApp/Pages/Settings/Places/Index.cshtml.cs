// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mileage.Domain.GetPlaces;

namespace Mileage.WebApp.Pages.Settings.Places;

[Authorize]
[ValidateAntiForgeryToken]
public sealed partial class IndexModel : PageModel
{
	private IDispatcher Dispatcher { get; }

	private ILog<IndexModel> Log { get; }

	public List<GetPlacesModel> Places { get; set; } = new();

	public IndexModel(IDispatcher dispatcher, ILog<IndexModel> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync()
	{
		var query = from u in User.GetUserId()
					from p in Dispatcher.DispatchAsync(new GetPlacesQuery(u))
					select p;

		await foreach (var places in query)
		{
			Places = places.ToList();
			return Page();
		}

		return new NoContentResult();
	}
}
