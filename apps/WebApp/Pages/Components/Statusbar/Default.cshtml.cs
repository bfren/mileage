// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;

namespace Mileage.WebApp.Pages.Components.Statusbar;

public sealed class StatusBarViewComponent : ViewComponent
{
	public IViewComponentResult Invoke() =>
		View();
}
