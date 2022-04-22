// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetPlaces;
using Mileage.Domain.SaveJourney;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Journeys;

public sealed class EditToPlacesModel : EditJourneyModalModel
{
	public List<GetPlacesModel> Places { get; set; } = new();

	public List<PlaceId> Selected { get; set; } = new();

	public EditToPlacesModel() : base("Destinations") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetEditToPlacesAsync(JourneyId journeyId) =>
		GetFieldAsync("ToPlaces", journeyId,
			x => Dispatcher.DispatchAsync(new GetPlacesQuery(x)),
			(j, v) => new EditToPlacesModel { Journey = j, Places = v.ToList(), Selected = j.ToPlaceIds.ToList() }
		);

	public Task<IActionResult> OnPostEditToPlacesAsync(UpdateJourneyToPlacesCommand journey) =>
		PostFieldAsync("ToPlaces", "To", journey, x => x.ToPlaceIds);
}
