// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.DeleteCar;
using Mileage.Domain.GetCar;
using Mileage.Persistence.Common.StrongIds;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Settings.Cars;

public sealed class DeleteModel : DeleteModalModel
{
	public GetCarModel Car { get; set; } = new();

	public DeleteModel() : base("Car") { }
}

public sealed partial class IndexModel
{
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
				none: r => Partial("Modals/ErrorModal", r)
			);
	}

	public async Task<IActionResult> OnPostDeletePartialAsync(DeleteCarCommand form)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.DispatchAsync(form with { UserId = u })
					select r;

		return await query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: _ => OnGetAsync(),
				none: r => Result.Error(r)
			);
	}
}
