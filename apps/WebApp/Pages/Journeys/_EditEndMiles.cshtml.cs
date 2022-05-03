// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.SaveJourney;
using Mileage.Persistence.Common.StrongIds;

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
			Result r when !r.Success =>
				r,

			_ =>
				Result.Create(true, Alert.Success("Done.")) with { RedirectTo = Url.Page("./Index", "Lists") }
		};
}
