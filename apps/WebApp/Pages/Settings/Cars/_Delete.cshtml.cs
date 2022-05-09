// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.CheckCarCanBeDeleted;
using Mileage.Domain.CheckCarCanBeDeleted.Messages;
using Mileage.Domain.DeleteCar;
using Mileage.Domain.GetCar;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.StrongIds;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Settings.Cars;

public sealed class DeleteModel : DeleteModalModel
{
	public CarModel Car { get; set; } = new();

	public DeleteModel() : base("Car") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetDeleteAsync(CarId carId)
	{
		// Create query
		var query = from userId in User.GetUserId()
					from op in Dispatcher.DispatchAsync(new CheckCarCanBeDeletedQuery(userId, carId))
					from car in Dispatcher.DispatchAsync(new GetCarQuery(userId, carId))
					select new { op, car };

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("_Delete", new DeleteModel { Car = x.car, Operation = x.op }),
				none: r => r switch
				{
					CarIsDefaultCarMsg =>
						Partial("_Delete", new DeleteModel
						{
							Operation = DeleteOperation.None,
							Reason = "it is the default car for new journeys"
						}),

					_ =>
						Partial("Modals/ErrorModal", r)
				}
			);
	}

	public Task<IActionResult> OnPostDeleteAsync(DeleteCarCommand car)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.DispatchAsync(car with { UserId = u })
					select r;

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: async x => x switch
				{
					true =>
						await OnGetAsync(),

					false =>
						Result.Error("Unable to delete car.")
				},
				none: r => Result.Error(r)
			);
	}
}
