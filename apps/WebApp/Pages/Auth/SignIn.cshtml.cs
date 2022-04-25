// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Logging;
using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Jeebs.Mvc.Auth.Functions;
using Jeebs.Mvc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mileage.WebApp.Pages.Auth;

[ValidateAntiForgeryToken]
public sealed class SignInModel : PageModel
{
	private IAuthDataProvider Auth { get; }

	private ILog Log { get; }

	[BindProperty]
	public Jeebs.Mvc.Auth.Models.SignInModel Form { get; set; } = new();

	public SignInModel(IAuthDataProvider auth, ILog<SignInModel> log) =>
		(Auth, Log) = (auth, log);

	public IActionResult OnGet(string? returnUrl)
	{
		Form = Form with { ReturnUrl = returnUrl };
		return Page();
	}

	public async Task<AuthResult> OnPostAsync()
	{
		Log.Dbg("{@Form}", Form);
		var result = await AuthF.DoSignInAsync(new(
			Model: Form,
			Auth: Auth,
			Log: Log,
			Url: Url,
			AddErrorAlert: TempData.AddErrorAlert,
			GetClaims: null,
			SignInAsync: HttpContext.SignInAsync,
			ValidateUserAsync: AuthF.ValidateUserAsync
		));

		if (result is AuthResult.SignedIn success)
		{
			return success with
			{
				Message = Alert.Success("You were signed in, redirecting..."),
				RedirectTo = Url.Page("/Journeys/Index", "Lists")
			};
		}

		return result;
	}
}
