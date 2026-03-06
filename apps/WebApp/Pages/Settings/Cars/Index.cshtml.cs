// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mileage.Domain.GetCars;
using Mileage.Domain.SaveCar;

namespace Mileage.WebApp.Pages.Settings.Cars;

[Authorize]
[ValidateAntiForgeryToken]
public sealed partial class IndexModel : PageModel
{
	private IDispatcher Dispatcher { get; }

	private ILog<IndexModel> Log { get; }

	public List<CarsModel> Cars { get; set; } = [];

	public IndexModel(IDispatcher dispatcher, ILog<IndexModel> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync()
	{
		var query = from u in User.GetUserId()
					from c in Dispatcher.SendAsync(new GetCarsQuery(u, true))
					select c;

		foreach (var cars in await query.Unsafe())
		{
			Cars = [.. cars];
		}

		return Page();
	}

	public Task<IActionResult> OnPostAsync(SaveCarQuery car)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.SendAsync(car with { UserId = u })
					select r;

		return query
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync(
				fOk: _ => OnGetAsync(),
				fFail: Op.Error
			);
	}
}
