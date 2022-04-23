// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetPlaces;
using Mileage.Domain.SaveSettings;

namespace Mileage.WebApp.Pages.Settings.Defaults;

public sealed class EditFromPlaceModel : EditSettingsModalModel
{
	public List<GetPlacesModel> Places { get; set; } = new();

	public EditFromPlaceModel() : base("Default Starting Place") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetEditFromPlaceAsync() =>
		GetFieldAsync("FromPlace",
			x => Dispatcher.DispatchAsync(new GetPlacesQuery(x)),
			(s, v) => new EditFromPlaceModel { Settings = s, Places = v.ToList() }
		);

	public Task<IActionResult> OnPostEditFromPlaceAsync(SaveSettingsCommand form) =>
		PostFieldAsync("FromPlace", "Default Starting Place", form, x => x.DefaultFromPlaceId);
}
