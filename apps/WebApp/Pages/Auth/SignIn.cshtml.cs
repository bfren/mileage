// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Mileage.WebApp.Pages.Auth;

public sealed class SignInModel : Jeebs.Mvc.Razor.Pages.Auth.SignInModel
{
	public SignInModel(IAuthDataProvider auth, ILog<SignInModel> log) : base(auth, log) =>
		SignInRedirect = () => Url.Page("/Journeys/Index", "Home");
}
