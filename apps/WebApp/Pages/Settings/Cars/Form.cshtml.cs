// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetCar;
using Mileage.Domain.SaveCar;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Settings.Cars;

public sealed partial class IndexModel
{
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
}
