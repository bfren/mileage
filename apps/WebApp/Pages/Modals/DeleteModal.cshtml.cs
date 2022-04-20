// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mileage.WebApp.Pages.Modals;

public abstract class DeleteModalModel : PageModel
{
	public string Title { get; set; }

	protected DeleteModalModel(string title) =>
		Title = title;
}