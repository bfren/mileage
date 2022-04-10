// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetFromPlace;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.Place;

public sealed record class PlaceModel(PlaceId Id, string Description, JourneyId? JourneyId)
{
	public static PlaceModel Blank =>
		new(new(), string.Empty, null);
}

public sealed class PlaceViewComponent : ViewComponent
{
	private IDispatcher Dispatcher { get; }

	private ILog Log { get; }

	public PlaceViewComponent(IDispatcher dispatcher, ILog<PlaceViewComponent> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public Task<IViewComponentResult> InvokeAsync(PlaceId placeId, JourneyId? journeyId)
	{
		Log.Dbg("Get place {PlaceId}.", placeId);
		return UserClaimsPrincipal
			.GetUserId()
			.BindAsync(
				x => Dispatcher.DispatchAsync(
					new GetFromPlaceQuery(x, placeId)
				)
			)
			.AuditAsync(
				none: r => Log.Err("Unable to get place: {Reason}", r)
			)
			.SwitchAsync(
				some: x => View(new PlaceModel(x.Id, x.Description, journeyId)),
				none: _ => (IViewComponentResult)View(PlaceModel.Blank)
			);
	}
}
