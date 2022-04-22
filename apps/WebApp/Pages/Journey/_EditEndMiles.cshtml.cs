// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetJourney;
using Mileage.Domain.SaveJourney;
using Mileage.Persistence.Common.StrongIds;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Journey;

[Authorize]
[ValidateAntiForgeryToken]
public sealed class EditEndMilesModel : EditModalModel
{
	public GetJourneyModel Journey { get; set; } = new();

	public IDispatcher Dispatcher { get; }

	public ILog<EditEndMilesModel> Log { get; }

	public EditEndMilesModel(IDispatcher dispatcher, ILog<EditEndMilesModel> log) : base("Start Miles") =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync(JourneyId journeyId)
	{
		Log.Vrb("Getting journey {JourneyId}.", journeyId);

		// Get settings for the current user
		var query = from u in User.GetUserId()
					from j in Dispatcher.DispatchAsync(new GetJourneyQuery(u, journeyId))
					select new
					{
						Journey = j
					};

		// Return page
		return (await query)
			.Switch<IActionResult>(
				some: x =>
				{
					Journey = x.Journey;
					return Page();
				},
				none: r => Partial("Modals/ErrorModal", r)
			);
	}

	public Task<IActionResult> OnPostAsync(UpdateJourneyEndMilesCommand journey)
	{
		Log.Vrb("Saving {Journey}.", journey);

		var query = from u in User.GetUserId()
					from r in Dispatcher.DispatchAsync(journey with { UserId = u })
					select r;

		var editUrl = Url.Page("EditEndMiles", values: new { journeyId = journey.Id.Value });

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync<bool, IActionResult>(
				some: x => x switch
				{
					true =>
						ViewComponent("Miles", new { label = "End", editUrl, miles = journey.EndMiles, journeyId = journey.Id }),

					false =>
						Result.Error("Unable to save end miles.")
				},
				none: r => Result.Error(r)
			);
	}
}
