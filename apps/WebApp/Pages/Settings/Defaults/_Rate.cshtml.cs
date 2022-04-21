// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetRates;
using Mileage.Domain.LoadSettings;
using Mileage.Domain.SaveSettings;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Settings.Defaults;

public sealed class RateModel : EditModalModel
{
	public Persistence.Common.Settings Settings { get; set; } = new();

	public List<GetRatesModel> Rates { get; set; } = new();

	public RateModel() : base("Default Rate") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetRateAsync()
	{
		Log.Vrb("Getting settings for {User}.", User.GetUserId());

		// Get settings for the current user
		var query = from u in User.GetUserId()
					from s in Dispatcher.DispatchAsync(new LoadSettingsQuery(u))
					from r in Dispatcher.DispatchAsync(new GetRatesQuery(u))
					select new
					{
						Settings = s,
						Rates = r
					};

		// Return page
		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("_EditDefaultCar", new RateModel { Settings = x.Settings, Rates = x.Rates.ToList() }),
				none: r => Partial("Modals/ErrorModal", r)
			);
	}

	public Task<IActionResult> OnPostRateAsync(SaveSettingsCommand form)
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
						ViewComponent("Rate", new { EditUrl = Url.Page("Index", "Rate"), CarId = form.Settings.DefaultRateId }),

					false =>
						Result.Error("Unable to save default rate.")
				},
				none: r => Result.Error(r)
			);
	}
}
