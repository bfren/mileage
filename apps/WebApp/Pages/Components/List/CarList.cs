// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Domain.GetCars;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.List;

public sealed class CarListViewComponent : ListSingleViewComponent<GetCarsModel, CarId>
{
	public CarListViewComponent() : base("car", x => x.Description,
		x => x.Description + (x.NumberPlate is string plate ? $" ({plate})" : string.Empty)
	)
	{ }
}
