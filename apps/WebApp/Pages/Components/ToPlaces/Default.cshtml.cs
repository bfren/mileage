// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetPlaces;
using Mileage.Persistence.Common.Ids;

namespace Mileage.WebApp.Pages.Components.ToPlaces;

public sealed record class ToPlacesModel(string Label, string? UpdateUrl, List<PlacesModel> Places, JourneyId JourneyId)
{
	public static ToPlacesModel Blank(string label, string? updateUrl, JourneyId journeyId) =>
		new(label, updateUrl, [], journeyId);
}

public sealed class ToPlacesViewComponent : ViewComponent
{
	private IDispatcher Dispatcher { get; }

	private ILog Log { get; }

	public ToPlacesViewComponent(IDispatcher dispatcher, ILog<ToPlacesViewComponent> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IViewComponentResult> InvokeAsync(string label, string updateUrl, List<PlaceId> value, JourneyId journeyId)
	{
		Log.Dbg("Get places {PlaceIds}.", value);
		return await UserClaimsPrincipal
			.GetUserId()
			.ToResult(nameof(ToPlacesViewComponent), nameof(InvokeAsync))
			.BindAsync(x => Dispatcher.SendAsync(new GetPlacesQuery(x, true)))
			.MapAsync(
				x => x.Where(p => value?.Contains(p.Id) == true).ToList()
			)
			.AuditAsync(fFail: r => Log.Err("Unable to get places: {Reason}.", r))
			.MatchAsync(
				fOk: x => View(new ToPlacesModel(label, updateUrl, x, journeyId)),
				fFail: _ => (IViewComponentResult)View(ToPlacesModel.Blank(label, updateUrl, journeyId))
			);
	}
}
