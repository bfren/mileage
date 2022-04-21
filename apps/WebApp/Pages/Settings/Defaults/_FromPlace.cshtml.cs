// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetPlaces;
using Mileage.Domain.LoadSettings;
using Mileage.Domain.SaveSettings;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Settings.Defaults;

public sealed class FromPlaceModel : EditModalModel
{
	public Persistence.Common.Settings Settings { get; set; } = new();

	public List<GetPlacesModel> Places { get; set; } = new();

	public FromPlaceModel() : base("Default Starting Place") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetFromPlaceAsync()
	{
		// Get settings for the current user
		var query = from u in User.GetUserId()
					from s in Dispatcher.DispatchAsync(new LoadSettingsQuery(u))
					from p in Dispatcher.DispatchAsync(new GetPlacesQuery(u))
					select new
					{
						Settings = s,
						Places = p
					};

		// Return page
		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("_FromPlace", new FromPlaceModel { Settings = x.Settings, Places = x.Places.ToList() }),
				none: r => Partial("Modals/ErrorModal", r)
			);
	}

	public Task<IActionResult> OnPostFromPlaceAsync(SaveSettingsCommand form)
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
						ViewComponent("FromPlace", new
						{
							label = "Default Starting Place",
							editUrl = Url.Page("Index", "FromPlace"),
							placeId = form.Settings.DefaultFromPlaceId
						}),

					false =>
						Result.Error("Unable to save default starting place.")
				},
				none: r => Result.Error(r)
			);
	}
}
