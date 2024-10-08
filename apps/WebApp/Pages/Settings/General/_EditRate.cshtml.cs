// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetRates;
using Mileage.Domain.SaveSettings;

namespace Mileage.WebApp.Pages.Settings.General;

public sealed class EditRateModel : EditSettingsModel
{
	public List<RatesModel> Rates { get; set; } = [];

	public EditRateModel() : base("Default Rate") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetEditRateAsync() =>
		GetFieldAsync("Rate",
			x => Dispatcher.DispatchAsync(new GetRatesQuery(x, false)),
			(s, v) => new EditRateModel { Settings = s, Rates = v.ToList() }
		);

	public Task<IActionResult> OnPostEditRateAsync(UpdateDefaultRateCommand settings) =>
		PostFieldAsync("Rate", "Default Rate", settings, x => x.DefaultRateId);
}
