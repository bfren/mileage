// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.Miles;

public sealed record class MilesModel(string Label, string? EditUrl, int Miles, JourneyId JourneyId);

public sealed class MilesViewComponent : ViewComponent
{
	public IViewComponentResult Invoke(string label, string editUrl, int value, JourneyId journeyId) =>
		View(new MilesModel(label, editUrl, value, journeyId));
}
