// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Collections.Generic;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data;
using Jeebs.Logging;
using Mileage.Persistence.Common;

namespace Mileage.Domain.GetExpensesReportMonths;

/// <summary>
/// Get expenses report months
/// </summary>
internal sealed class GetExpensesReportMonthsHandler : QueryHandler<GetExpensesReportMonthsQuery, IEnumerable<MonthModel>>
{
	private IDb Db { get; init; }

	private ILog<GetExpensesReportMonthsHandler> Log { get; init; }

	public GetExpensesReportMonthsHandler(IDb db, ILog<GetExpensesReportMonthsHandler> log) =>
		(Db, Log) = (db, log);

	public override Task<Maybe<IEnumerable<MonthModel>>> HandleAsync(GetExpensesReportMonthsQuery query)
	{
		var userId = query.UserId.Value;
		var months = 6;
		var sql = $"SELECT * FROM {Constants.Functions.GetExpensesReportRecentMonths}(@{nameof(userId)}, @{nameof(months)});";
		return Db.QueryAsync<MonthModel>(sql, new { userId, months }, System.Data.CommandType.Text);
	}
}