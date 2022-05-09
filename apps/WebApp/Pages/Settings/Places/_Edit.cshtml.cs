// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetPlace;
using Mileage.Persistence.Common.StrongIds;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Settings.Places;

public sealed class EditModel : UpdateModalModel
{
	public PlaceModel Place { get; set; } = new();

	public EditModel() : base("Place") { }
}

public sealed partial class IndexModel
{
	public async Task<PartialViewResult> OnGetEditAsync(PlaceId placeId)
	{
		// Create query
		var query = from u in User.GetUserId()
					from c in Dispatcher.DispatchAsync(new GetPlaceQuery(u, placeId))
					select c;

		return await query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("_Edit", new EditModel { Place = x }),
				none: r => Partial("Modals/ErrorModal", r)
			);
	}
}
