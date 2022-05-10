// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.SaveJourney;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Journeys;

public sealed class EditDayModel : EditJourneyModalModel
{
	public EditDayModel() : base("Date") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetEditDayAsync(JourneyId journeyId) =>
		GetFieldAsync("Day", journeyId, j => new EditDayModel { Journey = j });

	public async Task<IActionResult> OnPostEditDayAsync(UpdateJourneyDayCommand journey) =>
		await PostFieldAsync("Day", null, journey, x => x.Day) switch
		{
			Result r when !r.Success =>
				r,

			_ =>
				Result.Create(true, Alert.Success("Done.")) with { RedirectTo = "refresh" }
		};
}
