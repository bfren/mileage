// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

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

	public Task<IActionResult> OnPostEditDayAsync(UpdateJourneyDayCommand journey) =>
		PostFieldAsync("Day", null, journey, x => x.Day);
}
