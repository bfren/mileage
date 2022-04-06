// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Apps.Web.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Mileage.WebApp.Pages.Components.Header;

public sealed class Menu
{
	public readonly record struct Item(string Text, string Page);

	public IList<Item> Items { get; init; }

	public Menu() =>
		Items = new List<Item>
		{
			{ new("Home", "/") },
			{ new("Profile", "/profile") }
		};
}

public sealed record class HeaderModel(Menu Menu);

#if DEBUG
[ResponseCache(CacheProfileName = CacheProfiles.None)]
#else
[ResponseCache(CacheProfileName = CacheProfiles.Default)]
#endif
public sealed class HeaderViewComponent : ViewComponent
{
	public IViewComponentResult Invoke() =>
		View(new HeaderModel(
			Menu: new()
		));
}
