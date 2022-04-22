// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetRate;
using Mileage.Domain.SaveRate;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Settings.Rates;

public sealed partial class IndexModel
{
	public async Task<PartialViewResult> OnGetFormAsync(RateId? rateId)
	{
		// Return blank form
		if (rateId is null)
		{
			return Partial("_Form");
		}

		// Create query
		var query = from u in User.GetUserId()
					from c in Dispatcher.DispatchAsync(new GetRateQuery(u, rateId))
					select c;

		return await query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("_Form", x),
				none: () => Partial("_Form")
			);
	}

	public Task<IActionResult> OnPostFormAsync(SaveRateQuery form)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.DispatchAsync(form with { UserId = u })
					select r;

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: _ => OnGetAsync(),
				none: r => Result.Error(r)
			);
	}
}
