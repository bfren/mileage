// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetCar;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.Car;

public sealed record class CarModel(string? EditUrl, CarId Id, string Description, JourneyId? JourneyId)
{
	public static CarModel Blank(string? editUrl) =>
		new(editUrl, new(), string.Empty, null);
}

public sealed class CarViewComponent : ViewComponent
{
	private IDispatcher Dispatcher { get; }

	private ILog Log { get; }

	public CarViewComponent(IDispatcher dispatcher, ILog<CarViewComponent> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IViewComponentResult> InvokeAsync(string editUrl, CarId carId, JourneyId? journeyId)
	{
		if (carId is null)
		{
			return View(CarModel.Blank(editUrl));
		}

		Log.Dbg("Get car: {CarId}.", carId);
		return await UserClaimsPrincipal
			.GetUserId()
			.BindAsync(x => Dispatcher.DispatchAsync(new GetCarQuery(x, carId)))
			.AuditAsync(none: r => Log.Err("Unable to get car: {Reason}", r))
			.SwitchAsync(
				some: x => View(new CarModel(editUrl, x.Id, x.Description, journeyId)),
				none: _ => (IViewComponentResult)View(CarModel.Blank(editUrl))
			);
	}
}
