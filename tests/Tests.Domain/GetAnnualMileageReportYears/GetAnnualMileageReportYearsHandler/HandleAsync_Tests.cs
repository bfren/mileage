// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Jeebs.Data.Testing.Query;
using Jeebs.Logging;
using Mileage.Domain.GetGetAnnualMileageReportYears;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetAnnualMileageReportYears.GetAnnualMileageReportYearsHandler_Tests;

public sealed class HandleAsync_Tests : Abstracts.TestHandler
{
	private sealed class Setup : Setup<IJourneyRepository, JourneyEntity, JourneyId, GetAnnualMileageReportYearsHandler>
	{
		internal override GetAnnualMileageReportYearsHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);
	}

	private (GetAnnualMileageReportYearsHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	private sealed record class Vars(
		IJourneyRepository Repo,
		ILog<GetAnnualMileageReportYearsHandler> Log,
		GetAnnualMileageReportYearsHandler Handler,
		GetAnnualMileageReportYearsQuery Query
	);

	[Fact]
	public async Task Calls_Log_Vrb__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new GetAnnualMileageReportYearsQuery(LongId<AuthUserId>());
		v.Fluent.QueryAsync<DayModel>().ReturnsForAnyArgs(Create.None<IEnumerable<DayModel>>());

		// Act
		_ = await handler.HandleAsync(query);

		// Assert
		v.Log.Received().Vrb("Getting annual mileage report tax years for {User}.", query.UserId);
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var query = new GetAnnualMileageReportYearsQuery(userId);
		v.Fluent.QueryAsync<DayModel>().ReturnsForAnyArgs(Create.None<IEnumerable<DayModel>>());

		// Act
		_ = await handler.HandleAsync(query);

		// Assert
		v.Fluent.AssertCalls(
			c => FluentQueryHelper.AssertWhere<JourneyEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
			_ => { }
		);
	}

	[Fact]
	public async Task Returns_Sorted_Unique_Years()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var query = new GetAnnualMileageReportYearsQuery(userId);
		var days = new DayModel[]
		{
			new(new(2024, 1, 1)),
			new(new(2024, 3, 1)),
			new(new(2024, 5, 1)),
			new(new(2023, 7, 1)),
			new(new(2023, 9, 1)),
			new(new(2023, 2, 1)),
			new(new(2022, 4, 1)),
			new(new(2022, 6, 1)),
			new(new(2022, 8, 1))
		};
		v.Fluent.QueryAsync<DayModel>().ReturnsForAnyArgs(days);
		var expected = new[] { 2022, 2023, 2024, 2025 };

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var t = result.AssertSome();
		Assert.Collection(t,
			x => Assert.Equal(2024, x.StartYear),
			x => Assert.Equal(2023, x.StartYear),
			x => Assert.Equal(2022, x.StartYear),
			x => Assert.Equal(2021, x.StartYear)
		);
	}
}
