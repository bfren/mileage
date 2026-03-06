// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth;
using Jeebs.Auth.Data;
using Jeebs.Config.Web.Auth;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Jeebs.Mvc.Auth.Functions;
using Jeebs.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Mileage.WebApp.Pages.Auth;

public sealed class SignInModel : PageModel
{
	/// <summary>
	/// Auth Data provider
	/// </summary>
	private IAuthDataProvider Auth { get; init; }

	/// <summary>
	/// Auth JWT provider
	/// </summary>
	private IAuthJwtProvider Jwt { get; init; }

	/// <summary>
	/// Log
	/// </summary>
	private ILog Log { get; init; }

	/// <summary>
	/// Auth Config
	/// </summary>
	private AuthConfig Config { get; init; }

	/// <summary>
	/// Get application-specific claims for an authenticated user
	/// </summary>
	private AuthF.GetClaims? GetClaims { get; }

	/// <summary>
	/// Redirect here after a successful sign in
	/// </summary>
	private Func<string?> SignInRedirect { get; init; }

	/// <summary>
	/// Model to use for Sign In form
	/// </summary>
	public Jeebs.Mvc.Auth.Models.SignInModel Form { get; set; } = new();

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="auth"></param>
	/// <param name="jwt"></param>
	/// <param name="config"></param>
	/// <param name="log"></param>
	public SignInModel(IAuthDataProvider auth, IAuthJwtProvider jwt, IOptions<AuthConfig> config, ILog log) =>
		(Auth, Jwt, Log, Config, SignInRedirect) = (auth, jwt, log, config.Value, () => Url.Page("/Index"));

	/// <summary>
	/// Get standard sign in form
	/// </summary>
	public async Task<IActionResult> OnGetAsync() =>
		Page();

	/// <summary>
	/// Attempt sign in and return result
	/// </summary>
	/// <param name="form">Sign In form data</param>
	public async Task<AuthOp> OnPostFormAsync(Jeebs.Mvc.Auth.Models.SignInModel form)
	{
		Log.Dbg("Performing sign in using {@Form}.", form with { Password = "** REDACTED **" });
		var result = await AuthF.DoSignInAsync(new(form, Auth, Log, Url, GetClaims,
			SignInAsync: Config.Scheme switch
			{
				AuthScheme.Cookies =>
					user => AuthF.CreateCookieAsync(HttpContext, user, form.RememberMe, SignInRedirect()),

				AuthScheme.Jwt =>
					user => AuthF.CreateTokenAsync(Jwt, user, Log),

				_ =>
					async _ => new AuthOp.Denied()
			}
		));

		return result switch
		{
			AuthOp.SignedIn =>
				result with { Message = Alert.Success("You were signed in.") },

			_ =>
				result
		};
	}
}
