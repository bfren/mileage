// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetCars;
using Mileage.Domain.SaveJourney;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Journey;

public sealed class EditCarModel : EditJourneyModalModel
{
	public List<GetCarsModel> Cars { get; set; } = new();

	public EditCarModel() : base("Car") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetEditCarAsync(JourneyId journeyId) =>
		GetFieldAsync<IEnumerable<GetCarsModel>, EditCarModel>("Car", journeyId,
			u => Dispatcher.DispatchAsync(new GetCarsQuery(u)),
			(j, v) => new() { Journey = j, Cars = v.ToList() }
		);

	public Task<IActionResult> OnPostEditCarAsync(UpdateJourneyCarCommand journey) =>
		PostFieldAsync("Car", "Car", journey, x => x.CarId);
}
