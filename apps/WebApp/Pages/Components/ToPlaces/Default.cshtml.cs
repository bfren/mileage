// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Messages;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetPlaces;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.ToPlaces;

public sealed record class ToPlacesModel(string Label, string? EditUrl, List<GetPlacesModel> Places, JourneyId JourneyId)
{
	public static ToPlacesModel Blank(string label, string? editUrl, JourneyId journeyId) =>
		new(label, editUrl, new(), journeyId);
}

public sealed class ToPlacesViewComponent : ViewComponent
{
	private IDispatcher Dispatcher { get; }

	private ILog Log { get; }

	public ToPlacesViewComponent(IDispatcher dispatcher, ILog<ToPlacesViewComponent> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IViewComponentResult> InvokeAsync(string label, string editUrl, List<PlaceId> value, JourneyId journeyId)
	{
		Log.Dbg("Get places {PlaceIds}.", value);
		return await UserClaimsPrincipal
			.GetUserId()
			.BindAsync(x => Dispatcher.DispatchAsync(new GetPlacesQuery(x)))
			.MapAsync(
				x => x.Where(p => value?.Contains(p.Id) == true).ToList(),
				e => new M.UnableToGetToPlacesMsg(e)
			)
			.AuditAsync(none: r => Log.Err("Unable to get places: {Reason}", r))
			.SwitchAsync(
				some: x => View(new ToPlacesModel(label, editUrl, x, journeyId)),
				none: _ => (IViewComponentResult)View(ToPlacesModel.Blank(label, editUrl, journeyId))
			);
	}

	public static class M
	{
		public sealed record class UnableToGetToPlacesMsg(Exception Value) : ExceptionMsg;
	}
}
