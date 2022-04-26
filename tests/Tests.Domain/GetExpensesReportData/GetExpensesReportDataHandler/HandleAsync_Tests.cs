// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data;
using Jeebs.Logging;
using Mileage.Persistence.Common.Reports;

namespace Mileage.Domain.GetExpensesReportData.GetExpensesReportDataHandler_Tests;

public class HandleAsync_Tests
{
	private Vars Setup()
	{
		var db = Substitute.For<IDb>();
		var log = Substitute.For<ILog<GetExpensesReportDataHandler>>();
		var handler = new GetExpensesReportDataHandler(db, log);
		var query = new GetExpensesReportDataQuery(LongId<AuthUserId>(), Rnd.DateTime, Rnd.DateTime);
		return new(db, log, handler, query);
	}

	private sealed record class Vars(
		IDb Db,
		ILog<GetExpensesReportDataHandler> Log,
		GetExpensesReportDataHandler Handler,
		GetExpensesReportDataQuery Query
	);

	[Fact]
	public async Task Calls_Log_Vrb__With_Correct_Values()
	{
		// Arrange
		var v = Setup();

		// Act
		_ = await v.Handler.HandleAsync(v.Query);

		// Assert
		v.Log.Received().Vrb("Getting expenses report data for {Query}.", v.Query);
	}

	[Fact]
	public async Task Calls_Db_QueryAsync__With_Correct_Values()
	{
		// Arrange
		var v = Setup();
		var sql = "SELECT * FROM mileage.get_expenses_report_data(@UserId, @From, @To);";

		// Act
		_ = await v.Handler.HandleAsync(v.Query);

		// Assert
		await v.Db.Received().QueryAsync<ExpensesReportJourney>(sql, v.Query, System.Data.CommandType.Text);
	}

	[Fact]
	public async Task Calls_Db_QueryAsync__Returns_Result()
	{
		// Arrange
		var v = Setup();
		var list = Array.Empty<ExpensesReportJourney>();
		v.Db.QueryAsync<ExpensesReportJourney>(default!, default, default)
			.ReturnsForAnyArgs(list);

		// Act
		var result = await v.Handler.HandleAsync(v.Query);

		// Assert
		var some = result.AssertSome();
		Assert.Same(list, some);
	}
}
