// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mileage.WebApp.Pages.Modals;

public abstract class ModalModel(string title) : PageModel
{
	public string? Size { get; set => field = $"modal-{value}"; }

	public string Title { get; set; } = title;

	protected ModalModel(string title, string size) : this(title) =>
		Size = size;
}
