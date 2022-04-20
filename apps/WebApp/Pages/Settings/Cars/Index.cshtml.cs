// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Apps.Web.Constants;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mileage.Domain.DeleteCar;
using Mileage.Domain.GetCar;
using Mileage.Domain.GetCars;
using Mileage.Domain.SaveCar;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Settings.Cars;

#if DEBUG
[ResponseCache(CacheProfileName = CacheProfiles.None)]
#else
[ResponseCache(CacheProfileName = CacheProfiles.Default)]
#endif
[Authorize]
public sealed class IndexModel : PageModel
{
	private IDispatcher Dispatcher { get; }

	private ILog<IndexModel> Log { get; }

	public List<GetCarsModel> Cars { get; set; } = new();

	public IndexModel(IDispatcher dispatcher, ILog<IndexModel> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync()
	{
		var query = from u in User.GetUserId()
					from c in Dispatcher.DispatchAsync(new GetCarsQuery(u))
					select c;

		await foreach (var cars in query)
		{
			Cars = cars.ToList();
			return Page();
		}

		return new NoContentResult();
	}

	public async Task<PartialViewResult> OnGetFormPartialAsync(CarId? carId)
	{
		// Return blank form
		if (carId is null)
		{
			return Partial("Form");
		}

		// Create query
		var query = from u in User.GetUserId()
					from c in Dispatcher.DispatchAsync(new GetCarQuery(u, carId))
					select c;

		return await query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("Form", x),
				none: () => Partial("Form")
			);
	}

	public Task<IActionResult> OnPostFormPartialAsync(GetCarModel model)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.DispatchAsync(new SaveCarQuery(u, model))
					select r;

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: _ => OnGetAsync(),
				none: () => new NoContentResult()
			);
	}

	public async Task<PartialViewResult> OnGetDeletePartialAsync(CarId carId)
	{
		// Create query
		var query = from u in User.GetUserId()
					from c in Dispatcher.DispatchAsync(new GetCarQuery(u, carId))
					select c;

		return await query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("Delete", new DeleteModel { Car = x }),
				none: () => new PartialViewResult()
			);
	}

	public async Task<IActionResult> OnPostDeletePartialAsync(CarId carId)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.DispatchAsync(new DeleteCarCommand(u, carId))
					select r;

		return await query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: _ => OnGetAsync(),
				none: () => new NoContentResult()
			);
	}
}
