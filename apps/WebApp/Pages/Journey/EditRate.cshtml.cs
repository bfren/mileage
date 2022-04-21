// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetJourney;
using Mileage.Domain.GetRates;
using Mileage.Domain.SaveJourney;
using Mileage.Persistence.Common.StrongIds;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Journey;

[Authorize]
[ValidateAntiForgeryToken]
public sealed class EditRateModel : EditModalModel
{
	public GetJourneyModel Journey { get; set; } = new();

	public List<GetRatesModel> Rates { get; set; } = new();

	public IDispatcher Dispatcher { get; }

	public ILog<EditRateModel> Log { get; }

	public EditRateModel(IDispatcher dispatcher, ILog<EditRateModel> log) : base("Rate") =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync(JourneyId journeyId)
	{
		Log.Vrb("Getting journey {JourneyId}.", journeyId);

		// Get settings for the current user
		var query = from u in User.GetUserId()
					from j in Dispatcher.DispatchAsync(new GetJourneyQuery(u, journeyId))
					from r in Dispatcher.DispatchAsync(new GetRatesQuery(u))
					select new
					{
						Journey = j,
						Rates = r
					};

		// Return page
		return (await query)
			.Switch<IActionResult>(
				some: x =>
				{
					Journey = x.Journey;
					Rates = x.Rates.ToList();
					return Page();
				},
				none: r => Partial("Modals/ErrorModal", r)
			);
	}

	public Task<IActionResult> OnPostAsync(UpdateJourneyRateCommand journey)
	{
		Log.Vrb("Saving {Journey}.", journey);

		var query = from u in User.GetUserId()
					from r in Dispatcher.DispatchAsync(journey with { UserId = u })
					select r;

		var editUrl = Url.Page("EditRate", values: new { journeyId = journey.Id.Value });

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync<bool, IActionResult>(
				some: x => x switch
				{
					true =>
						ViewComponent("Rate", new { editUrl, journey.RateId, journeyId = journey.Id }),

					false =>
						Result.Error("Unable to save rate.")
				},
				none: r => Result.Error(r)
			);
	}
}
