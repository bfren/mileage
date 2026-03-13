// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetCar;
using Mileage.Persistence.Common.Ids;

namespace Mileage.WebApp.Pages.Components.Car;

public sealed record class CarModel(string Label, string? UpdateUrl, CarId Id, string Description, JourneyId? JourneyId)
{
	public static CarModel Blank(string label, string? updateUrl) =>
		new(label, updateUrl, new(), string.Empty, null);
}

public sealed class CarViewComponent : ViewComponent
{
	private IDispatcher Dispatcher { get; }

	private ILog Log { get; }

	public CarViewComponent(IDispatcher dispatcher, ILog<CarViewComponent> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IViewComponentResult> InvokeAsync(string label, string updateUrl, CarId value, JourneyId? journeyId)
	{
		if (value is null)
		{
			return View(CarModel.Blank(label, updateUrl));
		}

		Log.Dbg("Get car: {CarId}.", value);
		return await UserClaimsPrincipal
			.GetUserId()
			.ToResult(nameof(CarViewComponent), nameof(InvokeAsync))
			.BindAsync(x => Dispatcher.SendAsync(new GetCarQuery(x, value)))
			.AuditAsync(fFail: r => Log.Err("Unable to get car: {Reason}", r))
			.MatchAsync(
				fOk: x => View(new CarModel(label, updateUrl, x.Id, x.Description, journeyId)),
				fFail: _ => (IViewComponentResult)View(CarModel.Blank(label, updateUrl))
			);
	}
}
