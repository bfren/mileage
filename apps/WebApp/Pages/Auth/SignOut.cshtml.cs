// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Logging;

namespace Mileage.WebApp.Pages.Auth;

public sealed class SignOutModel : Jeebs.Mvc.Razor.Pages.Auth.SignOutModel
{
	public SignOutModel(ILog<SignOutModel> log) : base(log) { }
}
