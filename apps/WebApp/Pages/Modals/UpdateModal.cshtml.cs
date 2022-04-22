// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mileage.WebApp.Pages.Modals;

public abstract class UpdateModalModel : PageModel
{
	public string Title { get; set; }

	protected UpdateModalModel(string title) =>
		Title = title;
}
