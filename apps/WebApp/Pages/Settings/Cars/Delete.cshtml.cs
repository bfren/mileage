// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Domain.GetCar;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Settings.Cars;

public sealed class DeleteModel : DeleteModalModel
{
	public GetCarModel Car { get; set; } = new();

	public DeleteModel() : base("Car") { }
}
