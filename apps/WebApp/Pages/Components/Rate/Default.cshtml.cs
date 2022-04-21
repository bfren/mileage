// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetRate;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.Rate;

public sealed record class RateModel(string Label, string? EditUrl, RateId Id, float AmountPerMileGBP, JourneyId? JourneyId)
{
	public static RateModel Blank(string label, string? editUrl) =>
		new(label, editUrl, new(), 0f, null);
}

public sealed class RateViewComponent : ViewComponent
{
	private IDispatcher Dispatcher { get; }

	private ILog Log { get; }

	public RateViewComponent(IDispatcher dispatcher, ILog<RateViewComponent> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IViewComponentResult> InvokeAsync(string label, string editUrl, RateId rateId, JourneyId? journeyId)
	{
		if (rateId is null)
		{
			return View(RateModel.Blank(label, editUrl));
		}

		Log.Dbg("Get rate: {RateId}.", rateId);
		return await UserClaimsPrincipal
			.GetUserId()
			.BindAsync(x => Dispatcher.DispatchAsync(new GetRateQuery(x, rateId)))
			.AuditAsync(none: r => Log.Err("Unable to get rate: {Reason}", r))
			.SwitchAsync(
				some: x => View(new RateModel(label, editUrl, x.Id, x.AmountPerMileGBP, journeyId)),
				none: _ => (IViewComponentResult)View(RateModel.Blank(label, editUrl))
			);
	}
}
