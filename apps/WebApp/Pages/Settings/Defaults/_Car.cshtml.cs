// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetCars;
using Mileage.Domain.LoadSettings;
using Mileage.Domain.SaveSettings;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Settings.Defaults;

public sealed class CarModel : EditModalModel
{
	public Persistence.Common.Settings Settings { get; set; } = new();

	public List<GetCarsModel> Cars { get; set; } = new();

	public CarModel() : base("Default Car") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetCarAsync()
	{
		Log.Vrb("Getting settings for {User}.", User.GetUserId());

		// Get settings for the current user
		var query = from u in User.GetUserId()
					from s in Dispatcher.DispatchAsync(new LoadSettingsQuery(u))
					from c in Dispatcher.DispatchAsync(new GetCarsQuery(u))
					select new
					{
						Settings = s,
						Cars = c
					};

		// Return page
		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("_Car", new CarModel { Settings = x.Settings, Cars = x.Cars.ToList() }),
				none: r => Partial("Modals/ErrorModal", r)
			);
	}

	public Task<IActionResult> OnPostCarAsync(SaveSettingsCommand form)
	{
		Log.Vrb("Saving {Settings} for {User}.", form.Settings, User.GetUserId());

		var query = from u in User.GetUserId()
					from r in Dispatcher.DispatchAsync(form with { UserId = u })
					select r;

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync<bool, IActionResult>(
				some: x => x switch
				{
					true =>
						ViewComponent("Car", new
						{
							label = "Default Car",
							editUrl = Url.Page("Index", "Car"),
							carId = form.Settings.DefaultCarId
						}),

					false =>
						Result.Error("Unable to save default car.")
				},
				none: r => Result.Error(r)
			);
	}
}
