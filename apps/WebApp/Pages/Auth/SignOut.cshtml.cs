// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Auth.Functions;
using Jeebs.Mvc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mileage.WebApp.Pages.Auth;

[Authorize]
public sealed class SignOutModel : PageModel
{
	public async Task<IActionResult> OnGetAsync()
	{
		_ = await AuthF.DoSignOutAsync(new(
			TempData.AddInfoAlert,
			HttpContext.SignOutAsync
		));

		return Result.Create(true, Alert.Success("You were signed out.")) with { RedirectTo = Url.Page("./SignIn") };
	}
}
