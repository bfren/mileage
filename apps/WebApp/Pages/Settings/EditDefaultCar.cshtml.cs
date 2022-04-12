// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Messages;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetCars;
using Mileage.Domain.SaveSettings;
using Mileage.WebApp.Pages.Partials;

namespace Mileage.WebApp.Pages.Settings;

[ValidateAntiForgeryToken]
public sealed class EditDefaultCarModel : EditModalModel
{
	public Persistence.Common.Settings Settings { get; set; } = new();

	public List<GetCarsModel> Cars { get; set; } = new();

	public IDispatcher Dispatcher { get; }

	public ILog<EditDefaultCarModel> Log { get; }

	public EditDefaultCarModel(IDispatcher dispatcher, ILog<EditDefaultCarModel> log) : base("Default Car") =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync()
	{
		Log.Vrb("Getting settings for {User}.", User.GetUserId());

		// Get settings for the current user
		var query = from u in User.GetUserId()
					from s in Dispatcher.DispatchAsync(new Domain.LoadSettings.LoadSettingsQuery(u))
					from c in Dispatcher.DispatchAsync(new GetCarsQuery(u))
					select new
					{
						Settings = s,
						Cars = c
					};

		// Return page
		return (await query)
			.Switch<IActionResult>(
				some: x =>
				{
					Settings = x.Settings;
					Cars = x.Cars.ToList();
					return Page();
				},
				none: r => Partial("ErrorModal", r)
			);
	}

	public async Task<IActionResult> OnPostAsync(SaveSettingsCommand form)
	{
		Log.Vrb("Saving {Settings} for {User}.", form.Settings, User.GetUserId());

		var saveCar = from u in User.GetUserId()
					  from r in Dispatcher.DispatchAsync(form with { UserId = u })
					  select r;

		var result = await saveCar;
		return result
			.Audit(
				none: Log.Msg
			)
			.Switch<IActionResult>(
				some: x => x switch
				{
					true =>
						ViewComponent("Car", new { CarId = form.Settings.DefaultCarId }),

					false =>
						new EmptyResult()
				},
				none: _ => new EmptyResult()
			);
	}

	public static class M
	{
		public sealed record class UnableToSaveDefaultCarMsg : Msg;
	}
}
