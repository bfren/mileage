// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mileage.WebApp.Pages.Partials;

public abstract class EditModalModel : PageModel
{
	public string Title { get; init; }

	protected EditModalModel(string title) =>
		Title = title;
}
