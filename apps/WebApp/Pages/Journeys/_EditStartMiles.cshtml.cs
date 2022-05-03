// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.SaveJourney;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Journeys;

public sealed class EditStartMilesModel : EditJourneyModalModel
{
	public EditStartMilesModel() : base("Start Miles") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetEditStartMilesAsync(JourneyId journeyId) =>
		GetFieldAsync("StartMiles", journeyId, x => new EditStartMilesModel { Journey = x });

	public Task<IActionResult> OnPostEditStartMilesAsync(UpdateJourneyStartMilesCommand journey) =>
		PostFieldAsync("Miles", "Start", journey, x => x.StartMiles, "StartMiles");
}
