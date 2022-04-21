// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.Miles;

public sealed record class MilesModel(string Label, string? EditUrl, int Miles, JourneyId JourneyId);

public sealed class MilesViewComponent : ViewComponent
{
	public async Task<IViewComponentResult> InvokeAsync(string label, string editUrl, int miles, JourneyId journeyId) =>
		await Task.FromResult(View(new MilesModel(label, editUrl, miles, journeyId)));
}
