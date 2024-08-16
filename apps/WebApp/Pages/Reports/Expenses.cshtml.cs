// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Globalization;
using Jeebs;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mileage.Domain.GetExpensesReportData;
using Mileage.Persistence.Common.Reports;

namespace Mileage.WebApp.Pages.Reports;

[Authorize]
public sealed class ExpensesModel : PageModel
{
	private IDispatcher Dispatcher { get; init; }

	private ILog<ExpensesModel> Log { get; init; }

	public string Month { get; set; } = string.Empty;

	public List<ExpensesReportJourney> Journeys { get; set; } = [];

	public ExpensesModel(IDispatcher dispatcher, ILog<ExpensesModel> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync(int year, int month)
	{
		var date = new DateTime(year, month, 1);
		Month = date.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
		Log.Vrb("Generating expenses report for {Month}.", Month);

		var query = from u in User.GetUserId()
					from d in Dispatcher.DispatchAsync(new GetExpensesReportDataQuery(u, date.FirstDayOfMonth(), date.LastDayOfMonth()))
					select d;

		await foreach (var item in query.AuditAsync(none: Log.Msg))
		{
			Journeys = item.ToList();
		}

		return Page();
	}
}
