// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.SaveJourney;
using Mileage.Persistence.Common.Ids;

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
			Op x when !x.Success =>
				x,

			_ =>
				Op.Create("refresh", Alert.Success("Done."))
		};
}
