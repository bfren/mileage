// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mileage.Domain.GetAnnualMileageReportData;
using Mileage.Domain.GetGetAnnualMileageReportYears;
using Mileage.Persistence.Common.Reports;

namespace Mileage.WebApp.Pages.Reports;

[Authorize]
public sealed class AnnualMileageModel : PageModel
{
	private IDispatcher Dispatcher { get; init; }

	private ILog<AnnualMileageModel> Log { get; init; }

	public TaxYearModel TaxYear { get; set; } = new(0);

	public List<AnnualMileageReportJourney> Journeys { get; set; } = [];

	public AnnualMileageModel(IDispatcher dispatcher, ILog<AnnualMileageModel> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync(int start)
	{
		TaxYear = new(start);
		Log.Vrb("Generating annual mileage report for {Start}-{End}.", TaxYear.StartDate, TaxYear.EndDate);
		var query = from u in User.GetUserId()
					from d in Dispatcher.DispatchAsync(new GetAnnualMileageReportDataQuery(u, TaxYear.StartDate, TaxYear.EndDate))
					select d;

		await foreach (var item in query.AuditAsync(none: Log.Msg))
		{
			Journeys = item.ToList();
		}

		return Page();
	}
}
