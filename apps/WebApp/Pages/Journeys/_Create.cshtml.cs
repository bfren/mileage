// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Jeebs.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetCars;
using Mileage.Domain.GetLatestEndMiles;
using Mileage.Domain.GetPlaces;
using Mileage.Domain.GetRates;
using Mileage.Domain.LoadSettings;
using Mileage.Domain.SaveJourney;
using Mileage.Persistence.Common.Ids;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Journeys;

public sealed class CreateJourneyModel
{
	public DateTime Day { get; set; }

	public CarId? CarId { get; set; } = new();

	public uint StartMiles { get; set; }

	public uint? EndMiles { get; set; }

	public PlaceId? FromPlaceId { get; set; } = new();

	public PlaceId[] ToPlaceIds { get; set; } = [];

	public RateId? RateId { get; set; }
}

public sealed class CreateModel : ModalModel
{
	public CreateJourneyModel Journey { get; set; } = new();

	public List<CarsModel> Cars { get; set; } = [];

	public List<PlacesModel> Places { get; set; } = [];

	public List<RatesModel> Rates { get; set; } = [];

	public CreateModel() : base("Journey", "lg") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetCreateAsync()
	{
		var query = from u in User.GetUserId()
					from settings in Dispatcher.SendAsync(new LoadSettingsQuery(u))
					from cars in Dispatcher.SendAsync(new GetCarsQuery(u, false))
					from places in Dispatcher.SendAsync(new GetPlacesQuery(u, false))
					from rates in Dispatcher.SendAsync(new GetRatesQuery(u, false))
					from miles in Dispatcher.SendAsync(new GetLatestEndMilesQuery(u, settings.DefaultCarId))
					select new { settings, cars, places, rates, miles };

		return query
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync(
				fOk: x => Partial("_Create", new CreateModel
				{
					Journey = new()
					{
						Day = DateTime.Today,
						CarId = x.settings.DefaultCarId,
						FromPlaceId = x.settings.DefaultFromPlaceId,
						RateId = x.settings.DefaultRateId,
						StartMiles = x.miles
					},
					Cars = [.. x.cars],
					Places = [.. x.places],
					Rates = [.. x.rates]
				}),
				fFail: r => Partial("Modals/ErrorModal", r)
			);
	}

	public async Task<IActionResult> OnPostCreateAsync(SaveJourneyQuery journey)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.SendAsync(journey with { UserId = u })
					select r;

		return await query
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync(
				fOk: x => Op.Create("refresh", Alert.Success($"Created Journey {x}.")),
				fFail: r => Op.Error(r)
			);
	}
}
