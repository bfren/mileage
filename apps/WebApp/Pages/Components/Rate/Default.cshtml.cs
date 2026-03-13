// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetRate;
using Mileage.Persistence.Common.Ids;

namespace Mileage.WebApp.Pages.Components.Rate;

public sealed record class RateModel(string Label, string? UpdateUrl, RateId Id, float AmountPerMileGBP, JourneyId? JourneyId)
{
	public static RateModel Blank(string label, string? updateUrl) =>
		new(label, updateUrl, new(), 0f, null);
}

public sealed class RateViewComponent : ViewComponent
{
	private IDispatcher Dispatcher { get; }

	private ILog Log { get; }

	public RateViewComponent(IDispatcher dispatcher, ILog<RateViewComponent> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IViewComponentResult> InvokeAsync(string label, string updateUrl, RateId value, JourneyId? journeyId)
	{
		if (value is null)
		{
			return View(RateModel.Blank(label, updateUrl));
		}

		Log.Dbg("Get rate: {RateId}.", value);
		return await UserClaimsPrincipal
			.GetUserId()
			.ToResult(nameof(RateViewComponent), nameof(InvokeAsync))
			.BindAsync(x => Dispatcher.SendAsync(new GetRateQuery(x, value)))
			.AuditAsync(fFail: r => Log.Err("Unable to get rate: {Reason}", r))
			.MatchAsync(
				fOk: x => View(new RateModel(label, updateUrl, x.Id, x.AmountPerMileGBP, journeyId)),
				fFail: _ => (IViewComponentResult)View(RateModel.Blank(label, updateUrl))
			);
	}
}
