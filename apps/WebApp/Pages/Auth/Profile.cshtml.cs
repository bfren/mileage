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
using Mileage.Domain.InsertTestData;

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
					from p in Dispatcher.SendAsync(new GetUserProfileQuery(u))
					select p;

		foreach (var profile in await query.Unsafe())
		{
			Profile = profile;
			return Page();
		}

		return RedirectToPage("./SignOut");
	}

	public async Task<IActionResult> OnGetInsertTestDataAsync()
	{
		var query = from r in Dispatcher.SendAsync(new InsertTestDataCommand())
					select r;

		return await query
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync(
				fOk: x => Op.Create(x),
				fFail: r => Op.Error(r)
			);
	}
}
