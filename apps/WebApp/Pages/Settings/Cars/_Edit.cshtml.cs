// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetCar;
using Mileage.Persistence.Common.StrongIds;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Settings.Cars;

public sealed class EditModel : ModalModel
{
	public GetCarModel Car { get; set; } = new();

	public EditModel() : base("Car") { }
}

public sealed partial class IndexModel
{
	public async Task<PartialViewResult> OnGetEditAsync(CarId carId)
	{
		// Create query
		var query = from u in User.GetUserId()
					from c in Dispatcher.DispatchAsync(new GetCarQuery(u, carId))
					select c;

		return await query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("_Edit", new EditModel { Car = x }),
				none: r => Partial("Modals/ErrorModal", r)
			);
	}
}
