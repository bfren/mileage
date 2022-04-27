// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using StrongId;

namespace Mileage.WebApp.Pages.Components.List;

public sealed record class ListSingleItemModel(long Id, string Value, string Text);

public sealed record class ListSingleModel(
	string ListName,
	string Singular,
	bool AllowNull,
	List<ListSingleItemModel> Items,
	long? Selected
);

public abstract class ListSingleViewComponent<TModel, TId> : ViewComponent
	where TModel : IWithId<TId>
	where TId : LongId, new()
{
	public delegate string GetString(TModel model);

	private string Singular { get; }

	private GetString GetValue { get; }

	private GetString? GetText { get; }

	protected ListSingleViewComponent(string singular, GetString getValue) =>
		(Singular, GetValue) = (singular, getValue);

	protected ListSingleViewComponent(string singular, GetString getValue, GetString? getText) :
		this(singular, getValue) =>
		GetText = getText;

	public IViewComponentResult Invoke(string listName, bool allowNull, List<TModel> items, TId? selected)
	{
		var models = from i in items
					 orderby i.Id == selected descending
					 select new ListSingleItemModel(i.Id.Value, GetValue(i), (GetText ?? GetValue).Invoke(i));

		return View(
			"~/Pages/Components/List/ListSingle.cshtml",
			new ListSingleModel(listName, Singular, allowNull, models.ToList(), selected?.Value)
		);
	}
}
