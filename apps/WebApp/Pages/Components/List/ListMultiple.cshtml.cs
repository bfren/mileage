// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using StrongId;

namespace Mileage.WebApp.Pages.Components.List;

public sealed record class ListMultipleItemModel(long Id, string Value, string Text, bool Selected);

public sealed record class ListMultipleModel(
	string ListName,
	string Singular,
	List<ListMultipleItemModel> Items
);

public abstract class ListMultipleViewComponent<TModel, TId> : ViewComponent
	where TModel : IWithId<TId>
	where TId : LongId, new()
{
	public delegate string GetString(TModel model);

	private string Singular { get; }

	private GetString GetValue { get; }

	private GetString? GetText { get; }

	protected ListMultipleViewComponent(string singular, GetString getValue) =>
		(Singular, GetValue) = (singular, getValue);

	protected ListMultipleViewComponent(string singular, GetString getValue, GetString? getText) : this(singular, getValue) =>
		GetText = getText;

	public IViewComponentResult Invoke(string listName, List<TModel> items, TId[] selected)
	{
		var models = from i in items
					 let s = selected.Contains(i.Id)
					 orderby s descending
					 select new ListMultipleItemModel(i.Id.Value, GetValue(i), (GetText ?? GetValue).Invoke(i), s);

		return View(
			"~/Pages/Components/List/ListMultiple.cshtml",
			new ListMultipleModel(listName, Singular, models.ToList())
		);
	}
}
