// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetRate;
using Mileage.Persistence.Common.StrongIds;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Settings.Rates;

public sealed class EditModel : UpdateModalModel
{
	public RateModel Rate { get; set; } = new();

	public EditModel() : base("Rate") { }
}

public sealed partial class IndexModel
{
	public async Task<PartialViewResult> OnGetEditAsync(RateId rateId)
	{
		// Create query
		var query = from u in User.GetUserId()
					from c in Dispatcher.DispatchAsync(new GetRateQuery(u, rateId))
					select c;

		return await query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("_Edit", new EditModel { Rate = x }),
				none: r => Partial("Modals/ErrorModal", r)
			);
	}
}
