// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetLatestEndMiles;
using Mileage.Domain.LoadSettings;
using Mileage.Persistence.Common.StrongIds;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Journeys;

public sealed class CreateModel : CreateModalModel
{
	public DateTime Day { get; set; } = DateTime.Today;

	public CarId? CarId { get; set; }

	public PlaceId? FromPlaceId { get; set; }

	public RateId? RateId { get; set; }

	public uint StartMiles { get; set; }

	public CreateModel() : base("Journey") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetCreateAsync()
	{
		var query = from u in User.GetUserId()
					from s in Dispatcher.DispatchAsync(new LoadSettingsQuery(u))
					from m in Dispatcher.DispatchAsync(new GetLatestEndMilesQuery(u, s.DefaultCarId))
					select new
					{
						Settings = s,
						LatestEndMiles = m
					};

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("_Create", new CreateModel()
				{
					CarId = x.Settings.DefaultCarId,
					FromPlaceId = x.Settings.DefaultFromPlaceId,
					RateId = x.Settings.DefaultRateId,
					StartMiles = x.LatestEndMiles
				}),
				none: r => Partial("Modals/ErrorModal", r)
			);
	}
}
