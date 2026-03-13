// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.CheckRateCanBeDeleted;
using Mileage.Domain.DeleteRate;
using Mileage.Domain.GetRate;
using Mileage.Persistence.Common.Ids;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Settings.Rates;

public sealed class DeleteModel : DeleteModalModel
{
	public RateModel Rate { get; set; } = new();

	public DeleteModel() : base("Rate") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetDeleteAsync(RateId rateId)
	{
		// Create query
		var query = from userId in User.GetUserId()
					from op in Dispatcher.SendAsync(new CheckRateCanBeDeletedQuery(userId, rateId))
					from rate in Dispatcher.SendAsync(new GetRateQuery(userId, rateId))
					select new { op, rate };

		return query
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync(
				fOk: x => Partial("_Delete", new DeleteModel { Rate = x.rate, Operation = x.op }),
				fFail: r => Partial("Modals/ErrorModal", r)
			);
	}

	public Task<IActionResult> OnPostDeleteAsync(DeleteRateCommand rate)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.SendAsync(rate with { UserId = u })
					select r;

		return query
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync(
				fOk: async x => x switch
				{
					true =>
						await OnGetAsync(),

					false =>
						Op.Error("Unable to delete rate.")
				},
				fFail: Op.Error
			);
	}
}
