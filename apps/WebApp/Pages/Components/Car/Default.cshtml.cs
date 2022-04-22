// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetCar;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.Car;

public sealed record class CarModel(string Label, string? EditUrl, CarId Id, string Description, JourneyId? JourneyId)
{
	public static CarModel Blank(string label, string? editUrl) =>
		new(label, editUrl, new(), string.Empty, null);
}

public sealed class CarViewComponent : ViewComponent
{
	private IDispatcher Dispatcher { get; }

	private ILog Log { get; }

	public CarViewComponent(IDispatcher dispatcher, ILog<CarViewComponent> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IViewComponentResult> InvokeAsync(string label, string editUrl, CarId value, JourneyId? journeyId)
	{
		if (value is null)
		{
			return View(CarModel.Blank(label, editUrl));
		}

		Log.Dbg("Get car: {CarId}.", value);
		return await UserClaimsPrincipal
			.GetUserId()
			.BindAsync(x => Dispatcher.DispatchAsync(new GetCarQuery(x, value)))
			.AuditAsync(none: r => Log.Err("Unable to get car: {Reason}", r))
			.SwitchAsync(
				some: x => View(new CarModel(label, editUrl, x.Id, x.Description, journeyId)),
				none: _ => (IViewComponentResult)View(CarModel.Blank(label, editUrl))
			);
	}
}
