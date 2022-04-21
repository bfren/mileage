// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetPlace;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.Place;

public sealed record class PlaceModel(string? EditUrl, PlaceId Id, string Description, JourneyId? JourneyId)
{
	public static PlaceModel Blank(string? editUrl) =>
		new(editUrl, new(), string.Empty, null);
}

public sealed class PlaceViewComponent : ViewComponent
{
	private IDispatcher Dispatcher { get; }

	private ILog Log { get; }

	public PlaceViewComponent(IDispatcher dispatcher, ILog<PlaceViewComponent> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IViewComponentResult> InvokeAsync(string editUrl, PlaceId placeId, JourneyId? journeyId)
	{
		if (placeId is null)
		{
			return View(PlaceModel.Blank(editUrl));
		}

		Log.Dbg("Get place {PlaceId}.", placeId);
		return await UserClaimsPrincipal
			.GetUserId()
			.BindAsync(
				x => Dispatcher.DispatchAsync(
					new GetPlaceQuery(x, placeId)
				)
			)
			.AuditAsync(
				none: r => Log.Err("Unable to get place: {Reason}", r)
			)
			.SwitchAsync(
				some: x => View(new PlaceModel(editUrl, x.Id, x.Description, journeyId)),
				none: _ => (IViewComponentResult)View(PlaceModel.Blank(editUrl))
			);
	}
}
