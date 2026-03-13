// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.SaveJourney;
using Mileage.Persistence.Common.Ids;

namespace Mileage.WebApp.Pages.Journeys;

public sealed class EditEndMilesModel : EditJourneyModalModel
{
	public EditEndMilesModel() : base("End Miles") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetEditEndMilesAsync(JourneyId journeyId) =>
		GetFieldAsync("EndMiles", journeyId, x => new EditEndMilesModel { Journey = x });

	public async Task<IActionResult> OnPostEditEndMilesAsync(UpdateJourneyEndMilesCommand journey) =>
		await PostFieldAsync("Miles", "End", journey, x => x.EndMiles, "EndMiles") switch
		{
			Op x when !x.Success =>
				x,

			_ =>
				Op.Create("refresh", Alert.Success("Done."))
		};
}
