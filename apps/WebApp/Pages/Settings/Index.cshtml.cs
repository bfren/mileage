// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Apps.Web.Constants;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mileage.WebApp.Pages.Settings;

#if DEBUG
[ResponseCache(CacheProfileName = CacheProfiles.None)]
#else
[ResponseCache(CacheProfileName = CacheProfiles.Default)]
#endif
[Authorize]
public sealed class IndexModel : PageModel
{
	private IDispatcher Dispatcher { get; }

	private ILog<IndexModel> Log { get; }

	public Defaults.IndexModel Defaults { get; set; } = new();

	public IndexModel(IDispatcher dispatcher, ILog<IndexModel> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync()
	{
		// Get settings for the current user
		var query = from u in User.GetUserId()
					from s in Dispatcher.DispatchAsync(new Domain.LoadSettings.LoadSettingsQuery(u))
					select s;

		// Save and return page
		await foreach (var settings in query)
		{
			Defaults = new() { Settings = settings };
			return Page();
		}

		// No settings were found so log error and redirect to signout page
		_ = await query.AuditAsync(
			none: r =>
			{
				Log.Msg(r);
				TempData.AddErrorAlert(r);
			}
		);
		return RedirectToPage("/auth/signout");
	}
}
