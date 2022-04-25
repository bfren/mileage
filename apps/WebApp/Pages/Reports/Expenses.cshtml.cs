// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Globalization;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mileage.WebApp.Pages.Reports;

public sealed class ExpensesModel : PageModel
{
	private IDispatcher Dispatcher { get; init; }

	private ILog<ExpensesModel> Log { get; init; }

	public string Month { get; set; } = string.Empty;

	public ExpensesModel(IDispatcher dispatcher, ILog<ExpensesModel> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync(int year, int month)
	{
		var date = new DateTime(year, month, 1);
		Month = date.ToString("MMMM yyyy", CultureInfo.CurrentCulture);
		return Page();

	}
}
