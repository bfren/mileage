// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetJourney;
using Mileage.Domain.GetPlaces;
using Mileage.Domain.SaveJourney;
using Mileage.Domain.SavePlace;
using Mileage.Persistence.Common.Ids;

namespace Mileage.WebApp.Pages.Journeys;

public sealed class EditToPlacesModel : EditJourneyModalModel
{
	public List<PlacesModel> Places { get; set; } = [];

	public PlaceId[] Selected { get; set; } = [];

	public EditToPlacesModel() : base("Destinations", "lg") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetEditToPlacesAsync(JourneyId journeyId) =>
		GetFieldAsync("ToPlaces", journeyId,
			x => Dispatcher.SendAsync(new GetPlacesQuery(x, false)),
			(j, v) => new EditToPlacesModel { Journey = j, Places = [.. v], Selected = [.. j.ToPlaceIds] }
		);

	public Task<IActionResult> OnPostEditToPlacesAsync(UpdateJourneyToPlacesCommand journey) =>
		PostFieldAsync("ToPlaces", "To", journey, x => x.ToPlaceIds);

	public Task<IActionResult> OnPostCreateToPlaceAsync(AddNewItemToJourneyModel item) =>
		PostCreateItemAsync(
			u => Dispatcher.SendAsync(new SavePlaceQuery(u, item.Value)),
			(u, x) =>
			{
				var places = from p in Dispatcher.SendAsync(new GetJourneyQuery(u, item.Id))
							 select p.ToPlaceIds.WithItem(x);

				return places.MatchAsync(
					fOk: x => OnPostEditToPlacesAsync(new(u, item.Id, item.Version, [.. x])),
					fFail: r => Op.Error(r)
				);
			}
		);
}
