// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetCars;
using Mileage.Domain.SaveCar;
using Mileage.Domain.SaveJourney;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Journeys;

public sealed class EditCarModel : EditJourneyModalModel
{
	public List<CarsModel> Cars { get; set; } = new();

	public EditCarModel() : base("Car") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetEditCarAsync(JourneyId journeyId) =>
		GetFieldAsync<IEnumerable<CarsModel>, EditCarModel>("Car", journeyId,
			u => Dispatcher.DispatchAsync(new GetCarsQuery(u, false)),
			(j, v) => new() { Journey = j, Cars = v.ToList() }
		);

	public Task<IActionResult> OnPostEditCarAsync(UpdateJourneyCarCommand journey) =>
		PostFieldAsync("Car", "Car", journey, x => x.CarId);

	public Task<IActionResult> OnPostCreateCarAsync(AddNewItemToJourneyModel item) =>
		PostCreateItemAsync(
			u => Dispatcher.DispatchAsync(new SaveCarQuery(u, item.Value)),
			(u, x) => OnPostEditCarAsync(new(u, item.Id, item.Version, x))
		);
}
