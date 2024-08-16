// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetPlaces;
using Mileage.Domain.SaveSettings;

namespace Mileage.WebApp.Pages.Settings.General;

public sealed class EditFromPlaceModel : EditSettingsModel
{
	public List<PlacesModel> Places { get; set; } = [];

	public EditFromPlaceModel() : base("Default Starting Place") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetEditFromPlaceAsync() =>
		GetFieldAsync("FromPlace",
			x => Dispatcher.DispatchAsync(new GetPlacesQuery(x, false)),
			(s, v) => new EditFromPlaceModel { Settings = s, Places = v.ToList() }
		);

	public Task<IActionResult> OnPostEditFromPlaceAsync(UpdateDefaultFromPlaceCommand settings) =>
		PostFieldAsync("FromPlace", "Default Starting Place", settings, x => x.DefaultFromPlaceId);
}
