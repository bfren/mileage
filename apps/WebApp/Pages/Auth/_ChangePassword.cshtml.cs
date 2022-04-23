// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Models;
using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Jeebs.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetUserProfile;
using Mileage.Domain.SaveUserPassword;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Auth;

public sealed class ChangePasswordModel : UpdateModalModel
{
	public AuthChangePasswordModel Password { get; set; }

	public ChangePasswordModel(UserProfileModel model) : base("Password") =>
		Password = new(model.Id, model.Version, string.Empty, string.Empty, string.Empty);
}

public sealed partial class ProfileModel
{
	public Task<PartialViewResult> OnGetChangePasswordAsync()
	{
		var query = from u in User.GetUserId()
					from p in Dispatcher.DispatchAsync(new GetUserProfileQuery(u))
					select p;

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("_ChangePassword", new ChangePasswordModel(x)),
				none: r => Partial("Modals/ErrorModal", r)
			);
	}

	public async Task<IActionResult> OnPostChangePasswordAsync(SaveUserPasswordCommand password)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.DispatchAsync(password with { Id = u })
					select r;

		return await query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => x switch
				{
					true =>
						Result.Create(true, Alert.Success("Password changed successfully.")),

					false =>
						Result.Error("Unable to change password.")
				},
				none: r => Result.Error(r)
			);
	}
}
