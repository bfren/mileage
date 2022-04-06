// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Apps.Web;
using Jeebs.Mvc.Data;
using Microsoft.AspNetCore.Mvc;

namespace Mileage.WebApp;

public sealed class App : RazorApp
{
	protected override void ConfigureServicesMvcOptions(MvcOptions opt)
	{
		base.ConfigureServicesMvcOptions(opt);

		opt.AddStrongIdModelBinding();
	}
}
