// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Collections.Generic;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data;
using Jeebs.Logging;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.Reports;

namespace Mileage.Domain.GetAnnualMileageReportData;

/// <summary>
/// Get annual mileage report data
/// </summary>
internal sealed class GetAnnualMileageReportDataHandler : QueryHandler<GetAnnualMileageReportDataQuery, IEnumerable<AnnualMileageReportJourney>>
{
	private IDb Db { get; init; }

	private ILog<GetAnnualMileageReportDataHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="db"></param>
	/// <param name="log"></param>
	public GetAnnualMileageReportDataHandler(IDb db, ILog<GetAnnualMileageReportDataHandler> log) =>
		(Db, Log) = (db, log);

	/// <summary>
	/// Get the data for the expenses report requested by <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<IEnumerable<AnnualMileageReportJourney>>> HandleAsync(GetAnnualMileageReportDataQuery query)
	{
		Log.Vrb("Getting annual mileage report data for {Query}.", query);

		var sql = $"SELECT * FROM {Constants.Functions.GetAnnualMileageReportData}" +
			$"(@{nameof(query.UserId)}, @{nameof(query.From)}, @{nameof(query.To)});";
		return Db.QueryAsync<AnnualMileageReportJourney>(sql, query, System.Data.CommandType.Text);
	}
}
