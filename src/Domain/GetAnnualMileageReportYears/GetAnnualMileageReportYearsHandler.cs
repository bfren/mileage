// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetGetAnnualMileageReportYears;

/// <summary>
/// Get tax years
/// </summary>
internal sealed class GetAnnualMileageReportYearsHandler : QueryHandler<GetAnnualMileageReportYearsQuery, IEnumerable<TaxYearModel>>
{
	private IJourneyRepository Journey { get; init; }

	private ILog<GetAnnualMileageReportYearsHandler> Log { get; init; }

	/// <summary>
	/// Inject dependency
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="log"></param>
	public GetAnnualMileageReportYearsHandler(IJourneyRepository journey, ILog<GetAnnualMileageReportYearsHandler> log) =>
		(Journey, Log) = (journey, log);

	/// <summary>
	/// Get tax years for which journeys exist
	/// </summary>
	/// <param name="query"></param>
	public override async Task<Maybe<IEnumerable<TaxYearModel>>> HandleAsync(GetAnnualMileageReportYearsQuery query)
	{
		Log.Vrb("Getting tax years for which journeys exist.");

		// Select days from journey repo
		var days = await Journey
			.StartFluentQuery()
			.Where(j => j.UserId, Jeebs.Data.Enums.Compare.Equal, query.UserId)
			.QueryAsync<DayModel>();

		// Convert days to tax year values
		var (month, day) = (4, 6);
		return days.Map(
			x => from d in x
				 let year = d.Day.Year
				 let taxYear = new TaxYearModel(d.Day < new DateTime(year, month, day) ? year - 1 : year)
				 group taxYear by taxYear.StartYear into g
				 select g.First(),
			F.DefaultHandler
		);
	}
}
