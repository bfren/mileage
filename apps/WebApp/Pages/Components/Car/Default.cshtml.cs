// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetCar;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.Car;

public sealed record class CarModel(CarId Id, string Description, JourneyId? JourneyId)
{
	public static CarModel Blank =>
		new(new(), string.Empty, null);
}

public sealed class CarViewComponent : ViewComponent
{
	private IDispatcher Dispatcher { get; }

	private ILog Log { get; }

	public CarViewComponent(IDispatcher dispatcher, ILog<CarViewComponent> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public Task<IViewComponentResult> InvokeAsync(CarId carId, JourneyId? journeyId)
	{
		Log.Dbg("Get car {CarId}.", carId);
		return UserClaimsPrincipal
			.GetUserId()
			.BindAsync(
				x => Dispatcher.DispatchAsync(
					new GetCarQuery(x, carId)
				)
			)
			.AuditAsync(
				none: r => Log.Err("Unable to get car: {Reason}", r)
			)
			.SwitchAsync(
				some: x => View(new CarModel(x.Id, x.Description, journeyId)),
				none: _ => (IViewComponentResult)View(CarModel.Blank)
			);
	}
}
