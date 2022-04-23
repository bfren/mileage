// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetPlace;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.FromPlace;

public sealed record class FromPlaceModel(string Label, string? UpdateUrl, PlaceId Id, string Description, JourneyId? JourneyId)
{
	public static FromPlaceModel Blank(string label, string? updateUrl) =>
		new(label, updateUrl, new(), string.Empty, null);
}

public sealed class FromPlaceViewComponent : ViewComponent
{
	private IDispatcher Dispatcher { get; }

	private ILog Log { get; }

	public FromPlaceViewComponent(IDispatcher dispatcher, ILog<FromPlaceViewComponent> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IViewComponentResult> InvokeAsync(string label, string updateUrl, PlaceId value, JourneyId? journeyId)
	{
		if (value is null)
		{
			return View(FromPlaceModel.Blank(label, updateUrl));
		}

		Log.Dbg("Get place {PlaceId}.", value);
		return await UserClaimsPrincipal
			.GetUserId()
			.BindAsync(x => Dispatcher.DispatchAsync(new GetPlaceQuery(x, value)))
			.AuditAsync(none: r => Log.Err("Unable to get place: {Reason}", r))
			.SwitchAsync(
				some: x => View(new FromPlaceModel(label, updateUrl, x.Id, x.Description, journeyId)),
				none: _ => (IViewComponentResult)View(FromPlaceModel.Blank(label, updateUrl))
			);
	}
}
