// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Domain.GetCarsForUser;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.List;

public sealed class CarListViewComponent : ListViewComponent<GetCarsForUserModel, CarId>
{
	public CarListViewComponent() : base("car", x => x.Description) { }
}
