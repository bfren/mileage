// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using StrongId;

namespace Mileage.WebApp.Pages.Components.List;

public sealed record class ListItemModel(long Id, string Text);

public sealed record class ListModel(string ListName, string Singular, bool AllowNull, List<ListItemModel> Items, long? Selected);

public abstract class ListViewComponent<TModel, TId> : ViewComponent
	where TModel : IWithId<TId>
	where TId : LongId, new()
{
	private string Singular { get; }

	private Func<TModel, string> GetText { get; }

	protected ListViewComponent(string singular, Func<TModel, string> getText) =>
		(Singular, GetText) = (singular, getText);

	public IViewComponentResult Invoke(string listName, bool allowNull, List<TModel> items, TId? selected)
	{
		var models = from i in items select new ListItemModel(i.Id.Value, GetText(i));
		return View(
			"~/Pages/Components/List/List.cshtml",
			new ListModel(listName, Singular, allowNull, models.ToList(), selected?.Value)
		);
	}
}
