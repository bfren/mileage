// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Logging;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetPlacesForUser;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.PlaceList;

public sealed record class PlaceListModel(string ListName, bool AllowNull, List<GetPlacesForUserModel> Places, PlaceId? SelectedPlace);

public sealed class PlaceListViewComponent : ViewComponent
{
	private ILog Log { get; }

	public PlaceListViewComponent(ILog<PlaceListViewComponent> log) =>
		Log = log;

	public IViewComponentResult Invoke(string listName, bool allowNull, List<GetPlacesForUserModel> places, PlaceId? selectedPlace) =>
		View(new PlaceListModel(listName, allowNull, places, selectedPlace));
}
