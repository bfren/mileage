// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using System.Linq;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetGetAnnualMileageReportYears;

/// <summary>
/// Get tax years
/// </summary>
internal sealed class GetAnnualMileageReportYearsHandler : QueryHandler<GetAnnualMileageReportYearsQuery, IOrderedEnumerable<TaxYearModel>>
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
	public override async Task<Maybe<IOrderedEnumerable<TaxYearModel>>> HandleAsync(GetAnnualMileageReportYearsQuery query)
	{
		Log.Vrb("Getting annual mileage report tax years for {User}.", query.UserId);

		// Select days from journey repo
		var days = await Journey
			.StartFluentQuery()
			.Where(j => j.UserId, Jeebs.Data.Enums.Compare.Equal, query.UserId)
			.QueryAsync<DayModel>();

		// Convert days to tax year values
		return days.Map(
			x => from model in x
				 let taxYear = GetTaxYear(model.Day)
				 group taxYear by taxYear.StartYear into grp
				 select grp.First() into unique
				 orderby unique.StartYear descending
				 select unique,
			F.DefaultHandler
		);
	}

	/// <summary>
	/// Tax years begin on 6 April each year
	/// </summary>
	/// <param name="date">The date to check</param>
	/// <returns></returns>
	internal TaxYearModel GetTaxYear(DateTime date) =>
		(date < new DateTime(date.Year, 4, 6)) switch
		{
			true => // date is before the start of a new tax year
				new(date.Year - 1),

			false => // date is in the new tax year
				new(date.Year)
		};
}
