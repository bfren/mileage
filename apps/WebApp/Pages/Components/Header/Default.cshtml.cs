// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;

namespace Mileage.WebApp.Pages.Components.Header;

public sealed class Menu
{
	public readonly record struct Item(string Text, string Page);

	public IList<Item> Items { get; init; } = new List<Item>();

	public Menu() =>
		Items = new List<Item>
		{
			{ new("Journeys", "/Journeys/Index") },
			{ new("Reports", "/Reports/Index") },
			{ new("Settings", "/Settings/Index") },
			{ new("Profile", "/Auth/Profile") },
			{ new("Sign Out", "/Auth/Signout") }
		};
}

public sealed record class HeaderModel(Menu Menu);

public sealed class HeaderViewComponent : ViewComponent
{
	public IViewComponentResult Invoke() =>
		View(new HeaderModel(new()));
}
