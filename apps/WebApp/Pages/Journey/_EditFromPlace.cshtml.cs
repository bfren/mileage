// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetPlaces;
using Mileage.Domain.SaveJourney;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Journey;

public sealed class EditFromPlaceModel : EditJourneyModalModel
{
	public List<GetPlacesModel> Places { get; set; } = new();

	public EditFromPlaceModel() : base("Starting Place") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetEditFromPlaceAsync(JourneyId journeyId) =>
		GetFieldAsync("FromPlace", journeyId,
			u => Dispatcher.DispatchAsync(new GetPlacesQuery(u)),
			(j, v) => new EditFromPlaceModel { Journey = j, Places = v.ToList() }
		);

	public Task<IActionResult> OnPostEditFromPlaceAsync(UpdateJourneyFromPlaceCommand journey) =>
		PostFieldAsync("FromPlace", "From", journey, x => x.FromPlaceId);
}
