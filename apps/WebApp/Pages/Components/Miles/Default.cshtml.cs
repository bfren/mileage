// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.Miles;

public sealed record class MilesModel(string Label, string? UpdateUrl, int Miles, JourneyId JourneyId);

public sealed class MilesViewComponent : ViewComponent
{
	public IViewComponentResult Invoke(string label, string updateUrl, int value, JourneyId journeyId) =>
		View(new MilesModel(label, updateUrl, value, journeyId));
}
