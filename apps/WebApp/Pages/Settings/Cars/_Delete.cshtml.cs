// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.CheckCarCanBeDeleted;
using Mileage.Domain.DeleteCar;
using Mileage.Domain.GetCar;
using Mileage.Persistence.Common.Ids;
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
					from op in Dispatcher.SendAsync(new CheckCarCanBeDeletedQuery(userId, carId))
					from car in Dispatcher.SendAsync(new GetCarQuery(userId, carId))
					select new { op, car };

		return query
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync(
				fOk: x => Partial("_Delete", new DeleteModel { Car = x.car, Operation = x.op }),
				fFail: r => Partial("Modals/ErrorModal", r)
			);
	}

	public Task<IActionResult> OnPostDeleteAsync(DeleteCarCommand car)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.SendAsync(car with { UserId = u })
					select r;

		return query
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync(
				fOk: async x => x switch
				{
					true =>
						await OnGetAsync(),

					false =>
						Op.Error("Unable to delete car.")
				},
				fFail: Op.Error
			);
	}
}
