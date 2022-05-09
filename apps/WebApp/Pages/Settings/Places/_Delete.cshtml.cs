// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.CheckPlaceCanBeDeleted;
using Mileage.Domain.CheckPlaceCanBeDeleted.Messages;
using Mileage.Domain.DeletePlace;
using Mileage.Domain.GetPlace;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.StrongIds;
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
					from op in Dispatcher.DispatchAsync(new CheckPlaceCanBeDeletedQuery(userId, placeId))
					from place in Dispatcher.DispatchAsync(new GetPlaceQuery(userId, placeId))
					select new { op, place };

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("_Delete", new DeleteModel { Place = x.place, Operation = x.op }),
				none: r => r switch
				{
					PlaceIsDefaultFromPlaceMsg =>
						Partial("_Delete", new DeleteModel
						{
							Operation = DeleteOperation.None,
							Reason = "it is the default starting place for new journeys"
						}),

					_ =>
						Partial("Modals/ErrorModal", r)
				}
			);
	}

	public Task<IActionResult> OnPostDeleteAsync(DeletePlaceCommand place)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.DispatchAsync(place with { UserId = u })
					select r;

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: async x => x switch
				{
					true =>
						await OnGetAsync(),

					false =>
						Result.Error("Unable to delete place.")
				},
				none: r => Result.Error(r)
			);
	}
}
