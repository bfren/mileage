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
using Mileage.Persistence.Common.StrongIds;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Journeys;

public sealed class CreateJourneyModel
{
	public DateTime Day { get; set; }

	public CarId? CarId { get; set; } = new();

	public uint StartMiles { get; set; }

	public uint? EndMiles { get; set; }

	public PlaceId? FromPlaceId { get; set; } = new();

	public PlaceId[] ToPlaceIds { get; set; } = Array.Empty<PlaceId>();

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
					from settings in Dispatcher.DispatchAsync(new LoadSettingsQuery(u))
					from cars in Dispatcher.DispatchAsync(new GetCarsQuery(u, false))
					from places in Dispatcher.DispatchAsync(new GetPlacesQuery(u, false))
					from rates in Dispatcher.DispatchAsync(new GetRatesQuery(u, false))
					from miles in Dispatcher.DispatchAsync(new GetLatestEndMilesQuery(u, settings.DefaultCarId))
					select new { settings, cars, places, rates, miles };

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("_Create", new CreateModel
				{
					Journey = new()
					{
						Day = DateTime.Today,
						CarId = x.settings.DefaultCarId,
						FromPlaceId = x.settings.DefaultFromPlaceId,
						RateId = x.settings.DefaultRateId,
						StartMiles = x.miles
					},
					Cars = x.cars.ToList(),
					Places = x.places.ToList(),
					Rates = x.rates.ToList()
				}),
				none: r => Partial("Modals/ErrorModal", r)
			);
	}

	public async Task<IActionResult> OnPostCreateAsync(SaveJourneyQuery journey)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.DispatchAsync(journey with { UserId = u })
					select r;

		return await query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Result.Create("refresh", Alert.Success($"Created Journey {x}.")),
				none: r => Result.Error(r)
			);
	}
}
