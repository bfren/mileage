// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetUserProfile;
using Mileage.Domain.SaveUserProfile;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Auth;

public sealed class EditProfileModel : UpdateModalModel
{
	public UserProfileModel Profile { get; set; } = new();

	public EditProfileModel() : base("Profile") { }
}

public sealed partial class ProfileModel
{
	public Task<PartialViewResult> OnGetEditAsync()
	{
		var query = from u in User.GetUserId()
					from p in Dispatcher.SendAsync(new GetUserProfileQuery(u))
					select p;

		return query
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync(
				fOk: x => Partial("_EditProfile", new EditProfileModel { Profile = x }),
				fFail: r => Partial("Modals/ErrorModal", r)
			);
	}

	public Task<IActionResult> OnPostEditAsync(SaveUserProfileCommand profile)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.SendAsync(profile with { Id = u })
					select r;

		return query
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync(
				fOk: async x => x switch
				{
					true =>
						await OnGetAsync(),

					false =>
						Op.Error("Unable to save profile, please try again.")
				},
				fFail: Op.Error
			);
	}
}
