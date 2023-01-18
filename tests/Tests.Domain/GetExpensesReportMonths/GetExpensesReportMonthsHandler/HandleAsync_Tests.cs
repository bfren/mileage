// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data;
using Jeebs.Logging;
using Jeebs.Reflection;
using Mileage.Persistence.Common;

namespace Mileage.Domain.GetExpensesReportMonths.GetExpensesReportMonthsHandler_Tests;

public sealed class HandleAsync_Tests
{
	private Vars Setup()
	{
		var db = Substitute.For<IDb>();
		var log = Substitute.For<ILog<GetExpensesReportMonthsHandler>>();
		var handler = new GetExpensesReportMonthsHandler(db, log);
		var query = new GetExpensesReportMonthsQuery(LongId<AuthUserId>());
		return new(db, log, handler, query);
	}

	private sealed record class Vars(
		IDb Db,
		ILog<GetExpensesReportMonthsHandler> Log,
		GetExpensesReportMonthsHandler Handler,
		GetExpensesReportMonthsQuery Query
	);

	[Fact]
	public async Task Calls_Log_Vrb__With_Correct_Values()
	{
		// Arrange
		var v = Setup();

		// Act
		_ = await v.Handler.HandleAsync(v.Query);

		// Assert
		v.Log.Received().Vrb("Getting expenses report months for {User}.", v.Query.UserId);
	}

	[Fact]
	public async Task Calls_Db_QueryAsync__With_Correct_Values()
	{
		// Arrange
		var v = Setup();
		var sql = "SELECT * FROM mileage.get_expenses_report_recent_months(@userId, @months);";

		// Act
		_ = await v.Handler.HandleAsync(v.Query);

		// Assert
		await v.Db.Received().QueryAsync<MonthModel>(
			sql,
			Arg.Is<object>(x =>
				x.GetPropertyValue("userId").Equals(v.Query.UserId.Value)
				&& x.GetPropertyValue("months").Equals(Constants.ExpensesReportMonths)
			),
			System.Data.CommandType.Text
		);
	}

	[Fact]
	public async Task Calls_Db_QueryAsync__Returns_Result()
	{
		// Arrange
		var v = Setup();
		var list = Array.Empty<MonthModel>();
		v.Db.QueryAsync<MonthModel>(default!, default, default)
			.ReturnsForAnyArgs(list);

		// Act
		var result = await v.Handler.HandleAsync(v.Query);

		// Assert
		var some = result.AssertSome();
		Assert.Same(list, some);
	}
}
