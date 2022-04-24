// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mileage.Domain.GetUserProfile;
using Mileage.Domain.SaveUserProfile;

namespace Mileage.WebApp.Pages.Auth;

[Authorize]
[ValidateAntiForgeryToken]
public sealed partial class ProfileModel : PageModel
{
	public UserProfileModel Profile { get; set; } = new();

	private IDispatcher Dispatcher { get; }

	private ILog<ProfileModel> Log { get; }

	public ProfileModel(IDispatcher dispatcher, ILog<ProfileModel> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync()
	{
		var query = from u in User.GetUserId()
					from p in Dispatcher.DispatchAsync(new GetUserProfileQuery(u))
					select p;

		await foreach (var profile in query)
		{
			Profile = profile;
			return Page();
		}

		return RedirectToPage("./SignOut");
	}

	public async Task<IActionResult> OnPostAsync(SaveUserProfileCommand profile)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.DispatchAsync(profile with { Id = u })
					select r;

		return await query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: async x => x switch
				{
					true =>
						await OnGetAsync(),

					false =>
						Result.Error("Unable to save profile, please try again.")
				},
				none: r => Result.Error(r)
			);
	}
}
