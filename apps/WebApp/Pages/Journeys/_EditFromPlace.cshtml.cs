// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetPlaces;
using Mileage.Domain.SaveJourney;
using Mileage.Domain.SavePlace;
using Mileage.Persistence.Common.Ids;

namespace Mileage.WebApp.Pages.Journeys;

public sealed class EditFromPlaceModel : EditJourneyModalModel
{
	public List<PlacesModel> Places { get; set; } = [];

	public EditFromPlaceModel() : base("Starting Place") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetEditFromPlaceAsync(JourneyId journeyId) =>
		GetFieldAsync("FromPlace", journeyId,
			u => Dispatcher.SendAsync(new GetPlacesQuery(u, false)),
			(j, v) => new EditFromPlaceModel { Journey = j, Places = [.. v] }
		);

	public Task<IActionResult> OnPostEditFromPlaceAsync(UpdateJourneyFromPlaceCommand journey) =>
		PostFieldAsync("FromPlace", "From", journey, x => x.FromPlaceId);

	public Task<IActionResult> OnPostCreateFromPlaceAsync(AddNewItemToJourneyModel item) =>
		PostCreateItemAsync(
			u => Dispatcher.SendAsync(new SavePlaceQuery(u, item.Value)),
			(u, x) => OnPostEditFromPlaceAsync(new(u, item.Id, item.Version, x))
		);
}
