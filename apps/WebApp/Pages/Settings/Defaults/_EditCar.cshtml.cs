// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetCars;
using Mileage.Domain.SaveSettings;

namespace Mileage.WebApp.Pages.Settings.Defaults;

public sealed class EditCarModel : EditSettingsModalModel
{
	public List<GetCarsModel> Cars { get; set; } = new();

	public EditCarModel() : base("Default Car") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetEditCarAsync() =>
		GetFieldAsync("Car",
			x => Dispatcher.DispatchAsync(new GetCarsQuery(x)),
			(s, v) => new EditCarModel { Settings = s, Cars = v.ToList() }
		);

	public Task<IActionResult> OnPostEditCarAsync(SaveSettingsCommand form) =>
		PostFieldAsync("Car", "Default Car", form, x => x.DefaultCarId);
}
