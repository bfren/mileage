// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Messages;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetPlaces;
using Mileage.Domain.SaveSettings;
using Mileage.WebApp.Pages.Partials;

namespace Mileage.WebApp.Pages.Settings;

public sealed class EditDefaultFromPlaceModel : EditModalModel
{
	public Persistence.Common.Settings Settings { get; set; } = new();

	public List<GetPlacesModel> Places { get; set; } = new();

	public IDispatcher Dispatcher { get; }

	public ILog<EditDefaultFromPlaceModel> Log { get; }

	public EditDefaultFromPlaceModel(IDispatcher dispatcher, ILog<EditDefaultFromPlaceModel> log) : base("Default Starting Place") =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync()
	{
		// Get settings for the current user
		var query = from u in User.GetUserId()
					from s in Dispatcher.DispatchAsync(new Domain.LoadSettings.LoadSettingsQuery(u))
					from p in Dispatcher.DispatchAsync(new GetPlacesQuery(u))
					select new
					{
						Settings = s,
						Places = p
					};

		// Return page
		return (await query)
			.Switch<IActionResult>(
				some: x =>
				{
					Settings = x.Settings;
					Places = x.Places.ToList();
					return Page();
				},
				none: r => Partial("ErrorModal", r)
			);
	}

	public async Task<IActionResult> OnPostAsync(SaveSettingsCommand form)
	{
		Log.Vrb("Saving {Settings} for {User}.", form.Settings, User.GetUserId());

		var saveFromPlace = from u in User.GetUserId()
							from r in Dispatcher.DispatchAsync(form with { UserId = u })
							select r;

		var result = await saveFromPlace;
		return result
			.Audit(
				none: Log.Msg
			)
			.Switch<IActionResult>(
				some: x => x switch
				{
					true =>
						ViewComponent("Place", new { PlaceId = form.Settings.DefaultFromPlaceId }),

					false =>
						new EmptyResult()
				},
				none: _ => new EmptyResult()
			);
	}

	public static class M
	{
		public sealed record class UnableToSaveDefaultFromPlaceMsg : Msg;
	}
}
