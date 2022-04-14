// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mileage.WebApp.Pages.Settings.Defaults;

public class IndexModel : PageModel
{
	public Persistence.Common.Settings Settings { get; set; } = new();

	public void OnGet()
	{
	}
}
