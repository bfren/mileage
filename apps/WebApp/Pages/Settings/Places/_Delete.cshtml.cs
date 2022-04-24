// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.DeletePlace;
using Mileage.Domain.GetPlace;
using Mileage.Persistence.Common.StrongIds;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Settings.Places;

public sealed class DeleteModel : ModalModel
{
	public GetPlaceModel Place { get; set; } = new();

	public DeleteModel() : base("Place") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetDeleteAsync(PlaceId placeId)
	{
		// Create query
		var query = from u in User.GetUserId()
					from c in Dispatcher.DispatchAsync(new GetPlaceQuery(u, placeId))
					select c;

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("_Delete", new DeleteModel { Place = x }),
				none: r => Partial("Modals/ErrorModal", r)
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
