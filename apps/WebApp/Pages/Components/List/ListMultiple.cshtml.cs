// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using StrongId;

namespace Mileage.WebApp.Pages.Components.List;

public sealed record class ListMultipleItemModel(long Id, string Text, bool Selected);

public sealed record class ListMultipleModel(
	string ListName,
	string Singular,
	List<ListMultipleItemModel> Items
);

public abstract class ListMultipleViewComponent<TModel, TId> : ViewComponent
	where TModel : IWithId<TId>
	where TId : LongId, new()
{
	private string Singular { get; }

	private Func<TModel, string> GetText { get; }

	protected ListMultipleViewComponent(string singular, Func<TModel, string> getText) =>
		(Singular, GetText) = (singular, getText);

	public IViewComponentResult Invoke(string listName, List<TModel> items, List<TId> selected)
	{
		var models = from i in items
					 let s = selected.Contains(i.Id)
					 orderby s ascending
					 select new ListMultipleItemModel(i.Id.Value, GetText(i), s);

		return View(
			"~/Pages/Components/List/ListMultiple.cshtml",
			new ListMultipleModel(listName, Singular, models.ToList())
		);
	}
}
