// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

namespace Mileage.WebApp;

public static class HttpContextExtensions
{
	public static bool IsChildOf(this HttpContext @this, string page)
	{
		var path = @this.Request.Path.Value;
		return
			path is not null && path != "/" &&
			(
				path.StartsWith(page, StringComparison.InvariantCultureIgnoreCase)
				||
				page.StartsWith(path, StringComparison.InvariantCultureIgnoreCase)
			);
	}
}
