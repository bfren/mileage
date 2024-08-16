// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mileage.Domain.GetExpensesReportMonths;

namespace Mileage.WebApp.Pages.Reports;

[Authorize]
public sealed class IndexModel : PageModel
{
	private IDispatcher Dispatcher { get; init; }

	private ILog<IndexModel> Log { get; init; }

	public List<DateTime> ExpensesMonths { get; set; } = [];

	public IndexModel(IDispatcher dispatcher, ILog<IndexModel> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync()
	{
		var expensesQuery = from u in User.GetUserId()
							from m in Dispatcher.DispatchAsync(new GetExpensesReportMonthsQuery(u))
							select m;

		var query = from expenses in expensesQuery
					select new { expenses };

		_ = await query
			.AuditAsync(none: Log.Msg)
			.IfSomeAsync(
				x =>
				{
					ExpensesMonths = x.expenses.Select(y => new DateTime(y.Year, y.Month, 1)).ToList();
				}
			);

		return Page();
	}
}
