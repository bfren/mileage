// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mileage.WebApp.Pages.Settings.Places;

[Authorize]
[ValidateAntiForgeryToken]
public sealed class IndexModel : PageModel
{
	public void OnGet()
	{
	}
}
