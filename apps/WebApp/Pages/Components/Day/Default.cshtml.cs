// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.Day;

public sealed record class DayModel(string? UpdateUrl, DateTime Day, JourneyId JourneyId);

public sealed class DayViewComponent : ViewComponent
{
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable IDE0060 // Remove unused parameter
	public IViewComponentResult Invoke(string label, string updateUrl, DateTime value, JourneyId journeyId) =>
		View(new DayModel(updateUrl, value, journeyId));
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore RCS1163 // Unused parameter.
}
