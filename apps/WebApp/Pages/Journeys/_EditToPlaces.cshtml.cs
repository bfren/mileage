// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetJourney;
using Mileage.Domain.GetPlaces;
using Mileage.Domain.SaveJourney;
using Mileage.Domain.SavePlace;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Journeys;

public sealed class EditToPlacesModel : EditJourneyModalModel
{
	public List<GetPlacesModel> Places { get; set; } = new();

	public PlaceId[] Selected { get; set; } = Array.Empty<PlaceId>();

	public EditToPlacesModel() : base("Destinations", "lg") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetEditToPlacesAsync(JourneyId journeyId) =>
		GetFieldAsync("ToPlaces", journeyId,
			x => Dispatcher.DispatchAsync(new GetPlacesQuery(x)),
			(j, v) => new EditToPlacesModel { Journey = j, Places = v.ToList(), Selected = j.ToPlaceIds.ToArray() }
		);

	public Task<IActionResult> OnPostEditToPlacesAsync(UpdateJourneyToPlacesCommand journey) =>
		PostFieldAsync("ToPlaces", "To", journey, x => x.ToPlaceIds);

	public Task<IActionResult> OnPostCreateToPlaceAsync(AddNewItemToJourneyModel item) =>
		PostCreateItemAsync(
			u => Dispatcher.DispatchAsync(new SavePlaceQuery(u, item.Value)),
			(u, x) =>
			{
				var places = from p in Dispatcher.DispatchAsync(new GetJourneyQuery(u, item.Id))
							 select p.ToPlaceIds.WithItem(x);

				return places.SwitchAsync(
					some: x => OnPostEditToPlacesAsync(new(u, item.Id, item.Version, x.ToList())),
					none: r => Result.Error(r)
				);
			}
		);
}
