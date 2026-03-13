// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mileage.WebApp.Pages.Auth;

public sealed class IndexModel : PageModel
{
	/// <summary>
	/// Log.
	/// </summary>
	private ILog Log { get; init; }

	/// <summary>
	/// Inject dependencies.
	/// </summary>
	/// <param name="log">ILog.</param>
	public IndexModel(ILog<IndexModel> log) =>
		Log = log;

	public IActionResult OnGet() =>
		RedirectToPage("Profile");
}
