// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetCars;
using Mileage.Domain.GetJourney;
using Mileage.Domain.SaveJourney;
using Mileage.Persistence.Common.StrongIds;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Journey;

[Authorize]
[ValidateAntiForgeryToken]
public sealed class EditCarModel : EditModalModel
{
	public GetJourneyModel Journey { get; set; } = new();

	public List<GetCarsModel> Cars { get; set; } = new();

	public IDispatcher Dispatcher { get; }

	public ILog<EditCarModel> Log { get; }

	public EditCarModel(IDispatcher dispatcher, ILog<EditCarModel> log) : base("Car") =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync(JourneyId journeyId)
	{
		Log.Vrb("Getting journey {JourneyId}.", journeyId);

		// Get settings for the current user
		var query = from u in User.GetUserId()
					from j in Dispatcher.DispatchAsync(new GetJourneyQuery(u, journeyId))
					from c in Dispatcher.DispatchAsync(new GetCarsQuery(u))
					select new
					{
						Journey = j,
						Cars = c
					};

		// Return page
		return (await query)
			.Switch<IActionResult>(
				some: x =>
				{
					Journey = x.Journey;
					Cars = x.Cars.ToList();
					return Page();
				},
				none: r => Partial("Modals/ErrorModal", r)
			);
	}

	public async Task<IActionResult> OnPostAsync(UpdateJourneyCarCommand journey)
	{
		Log.Vrb("Saving {Journey}.", journey);

		var saveCar = from u in User.GetUserId()
					  from r in Dispatcher.DispatchAsync(journey with { UserId = u })
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
						ViewComponent("Car", new { journey.CarId, journeyId = journey.Id }),

					false =>
						new EmptyResult()
				},
				none: _ => new EmptyResult()
			);
	}
}
