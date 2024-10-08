// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetRates;
using Mileage.Domain.SaveJourney;
using Mileage.Domain.SaveRate;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Journeys;

public sealed class EditRateModel : EditJourneyModalModel
{
	public List<RatesModel> Rates { get; set; } = [];

	public EditRateModel() : base("Rate") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetEditRateAsync(JourneyId journeyId) =>
		GetFieldAsync("Rate", journeyId,
			u => Dispatcher.DispatchAsync(new GetRatesQuery(u, false)),
			(j, v) => new EditRateModel { Journey = j, Rates = v.ToList() }
		);

	public Task<IActionResult> OnPostEditRateAsync(UpdateJourneyRateCommand journey) =>
		PostFieldAsync("Rate", "Rate", journey, x => x.RateId);

	public Task<IActionResult> OnPostCreateRateAsync(AddNewItemToJourneyModel item) =>
		PostCreateItemAsync(
			u => Dispatcher.DispatchAsync(new SaveRateQuery(u, float.Parse(item.Value, CultureInfo.InvariantCulture))),
			(u, x) => OnPostEditRateAsync(new(u, item.Id, item.Version, x))
		);
}
