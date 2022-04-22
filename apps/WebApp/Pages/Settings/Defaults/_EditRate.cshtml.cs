// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetRates;
using Mileage.Domain.SaveSettings;

namespace Mileage.WebApp.Pages.Settings.Defaults;

public sealed class EditRateModel : EditSettingsModalModel
{
	public List<GetRatesModel> Rates { get; set; } = new();

	public EditRateModel() : base("Default Rate") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetEditRateAsync() =>
		GetFieldAsync("Rate",
			x => Dispatcher.DispatchAsync(new GetRatesQuery(x)),
			(s, v) => new EditRateModel { Settings = s, Rates = v.ToList() }
		);

	public Task<IActionResult> OnPostEditRateAsync(SaveSettingsCommand form) =>
		PostFieldAsync("Rate", "Default Rate", form, x => x.DefaultRateId);
}
