// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.CheckPlaceCanBeDeleted;
using Mileage.Domain.DeletePlace;
using Mileage.Domain.GetPlace;
using Mileage.Persistence.Common.Ids;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Settings.Places;

public sealed class DeleteModel : DeleteModalModel
{
	public PlaceModel Place { get; set; } = new();

	public DeleteModel() : base("Place") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetDeleteAsync(PlaceId placeId)
	{
		// Create query
		var query = from userId in User.GetUserId()
					from op in Dispatcher.SendAsync(new CheckPlaceCanBeDeletedQuery(userId, placeId))
					from place in Dispatcher.SendAsync(new GetPlaceQuery(userId, placeId))
					select new { op, place };

		return query
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync(
				fOk: x => Partial("_Delete", new DeleteModel { Place = x.place, Operation = x.op }),
				fFail: r => Partial("Modals/ErrorModal", r)
			);
	}

	public Task<IActionResult> OnPostDeleteAsync(DeletePlaceCommand place)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.SendAsync(place with { UserId = u })
					select r;

		return query
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync(
				fOk: async x => x switch
				{
					true =>
						await OnGetAsync(),

					false =>
						Op.Error("Unable to delete place.")
				},
				fFail: Op.Error
			);
	}
}
