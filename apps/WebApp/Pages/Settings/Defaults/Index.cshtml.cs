// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Security.Claims;
using Jeebs.Apps.Web.Constants;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mileage.WebApp.Pages.Settings.Defaults;

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

	public Persistence.Common.Settings Settings { get; set; } = new();

	public IndexModel(IDispatcher dispatcher, ILog<IndexModel> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync()
	{
		Console.WriteLine("=============== HI! ===============");

		// Get settings and return page
		await foreach (var settings in GetSettingsAsync(User, Dispatcher))
		{
			Settings = settings;
			return Page();
		}

		return new NoContentResult();
	}

	/// <summary>
	/// Get settings for the current user
	/// </summary>
	/// <param name="user"></param>
	/// <param name="dispatcher"></param>
	internal static Task<Maybe<Persistence.Common.Settings>> GetSettingsAsync(ClaimsPrincipal user, IDispatcher dispatcher) =>
		from u in user.GetUserId()
		from s in dispatcher.DispatchAsync(new Domain.LoadSettings.LoadSettingsQuery(u))
		select s;
}
