// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Collections.Generic;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data;
using Jeebs.Logging;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.Reports;

namespace Mileage.Domain.GetExpensesReportData;

/// <summary>
/// Get expenses report data
/// </summary>
internal sealed class GetExpensesReportDataHandler : QueryHandler<GetExpensesReportDataQuery, IEnumerable<ExpensesReportJourney>>
{
	private IDb Db { get; init; }

	private ILog<GetExpensesReportDataHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="db"></param>
	/// <param name="log"></param>
	public GetExpensesReportDataHandler(IDb db, ILog<GetExpensesReportDataHandler> log) =>
		(Db, Log) = (db, log);

	/// <summary>
	/// Get the data for the expenses report requested by <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<IEnumerable<ExpensesReportJourney>>> HandleAsync(GetExpensesReportDataQuery query)
	{
		Log.Vrb("Getting expenses report data for {Query}.", query);

		var sql = $"SELECT * FROM {Constants.Functions.GetExpensesReportData}" +
			$"(@{nameof(query.UserId)}, @{nameof(query.From)}, @{nameof(query.To)});";
		return Db.QueryAsync<ExpensesReportJourney>(sql, query, System.Data.CommandType.Text);
	}
}
