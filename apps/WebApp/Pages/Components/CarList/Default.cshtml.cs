// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Logging;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetCarsForUser;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.CarList;

public sealed record class CarListModel(string ListName, bool AllowNull, List<GetCarsForUserModel> Cars, CarId? SelectedCar);

public sealed class CarListViewComponent : ViewComponent
{
	private ILog Log { get; }

	public CarListViewComponent(ILog<CarListViewComponent> log) =>
		Log = log;

	public IViewComponentResult Invoke(string listName, bool allowNull, List<GetCarsForUserModel> cars, CarId? selectedCar) =>
		View(new CarListModel(listName, allowNull, cars, selectedCar));
}
