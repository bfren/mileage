// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.Day;

public sealed record class DayModel(string? EditUrl, DateTime Day, JourneyId? JourneyId)
{
	public static DayModel Blank(string? editUrl) =>
		new(editUrl, DateTime.Today, null);
}

public sealed class DayViewComponent : ViewComponent
{
	public async Task<IViewComponentResult> InvokeAsync(string editUrl, DateTime day, JourneyId? journeyId) =>
		await Task.FromResult(View(new DayModel(editUrl, day, journeyId)));
}
