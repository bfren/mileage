// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.CheckRateCanBeDeleted;
using Mileage.Domain.CheckRateCanBeDeleted.Messages;
using Mileage.Domain.DeleteRate;
using Mileage.Domain.GetRate;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.StrongIds;
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
					from op in Dispatcher.DispatchAsync(new CheckRateCanBeDeletedQuery(userId, rateId))
					from rate in Dispatcher.DispatchAsync(new GetRateQuery(userId, rateId))
					select new { op, rate };

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("_Delete", new DeleteModel { Rate = x.rate, Operation = x.op }),
				none: r => r switch
				{
					RateIsDefaultRateMsg =>
						Partial("_Delete", new DeleteModel
						{
							Operation = DeleteOperation.None,
							Reason = "it is the default rate for new journeys"
						}),

					_ =>
						Partial("Modals/ErrorModal", r)
				}
			);
	}

	public Task<IActionResult> OnPostDeleteAsync(DeleteRateCommand rate)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.DispatchAsync(rate with { UserId = u })
					select r;

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: async x => x switch
				{
					true =>
						await OnGetAsync(),

					false =>
						Result.Error("Unable to delete rate.")
				},
				none: r => Result.Error(r)
			);
	}
}
