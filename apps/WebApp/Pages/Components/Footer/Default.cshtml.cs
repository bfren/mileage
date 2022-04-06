// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Reflection;
using Jeebs.Apps.Web.Constants;
using Jeebs.Functions;
using Microsoft.AspNetCore.Mvc;

namespace Mileage.WebApp.Pages.Components.Footer;

public sealed record class FooterModel(string Build, DateTime LastModified);

#if DEBUG
[ResponseCache(CacheProfileName = CacheProfiles.None)]
#else
[ResponseCache(CacheProfileName = CacheProfiles.Default)]
#endif
public sealed class FooterViewComponent : ViewComponent
{
	public async Task<IViewComponentResult> InvokeAsync() =>
		View(new FooterModel(
			Build: await VersionF.GetJeebsVersionAsync(),
			LastModified: File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location)
		));
}
