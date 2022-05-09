// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetPlaces;
using Mileage.Domain.SaveJourney;
using Mileage.Domain.SavePlace;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Journeys;

public sealed class EditFromPlaceModel : EditJourneyModalModel
{
	public List<PlacesModel> Places { get; set; } = new();

	public EditFromPlaceModel() : base("Starting Place") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetEditFromPlaceAsync(JourneyId journeyId) =>
		GetFieldAsync("FromPlace", journeyId,
			u => Dispatcher.DispatchAsync(new GetPlacesQuery(u, false)),
			(j, v) => new EditFromPlaceModel { Journey = j, Places = v.ToList() }
		);

	public Task<IActionResult> OnPostEditFromPlaceAsync(UpdateJourneyFromPlaceCommand journey) =>
		PostFieldAsync("FromPlace", "From", journey, x => x.FromPlaceId);

	public Task<IActionResult> OnPostCreateFromPlaceAsync(AddNewItemToJourneyModel item) =>
		PostCreateItemAsync(
			u => Dispatcher.DispatchAsync(new SavePlaceQuery(u, item.Value)),
			(u, x) => OnPostEditFromPlaceAsync(new(u, item.Id, item.Version, x))
		);
}
