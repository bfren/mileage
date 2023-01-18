// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Linq.Expressions;
using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Jeebs.Data.Testing.Query;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetJourneys.GetJourneysHandler_Tests;

public sealed class HandleAsync_Tests : Abstracts.TestHandler
{
	private sealed class Setup : Setup<IJourneyRepository, JourneyEntity, JourneyId, GetJourneysHandler>
	{
		internal override GetJourneysHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);
	}

	private (GetJourneysHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task Logs_To_Vrb__With_Query()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var start = DateTime.SpecifyKind(Rnd.DateTime, DateTimeKind.Unspecified);
		var end = DateTime.SpecifyKind(Rnd.DateTime, DateTimeKind.Unspecified);
		var query = new GetJourneysQuery(userId, start, end);

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Log.Received().Vrb("Getting Journeys for {User} between {Start} and {End}.", userId.Value, start, end);
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var start = DateTime.SpecifyKind(Rnd.DateTime, DateTimeKind.Unspecified);
		var end = DateTime.SpecifyKind(Rnd.DateTime, DateTimeKind.Unspecified);
		var query = new GetJourneysQuery(userId, start, end);

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Fluent.AssertCalls(
			c => FluentQueryHelper.AssertWhere<JourneyEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
			c => FluentQueryHelper.AssertWhere<JourneyEntity, DateTime>(c, x => x.Day, Compare.MoreThanOrEqual, start),
			c => FluentQueryHelper.AssertWhere<JourneyEntity, DateTime>(c, x => x.Day, Compare.LessThanOrEqual, end),
			_ => { },
			_ => { },
			_ => { }
		);
	}

	[Fact]
	public async Task Calls_FluentQuery_Sort__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new GetJourneysQuery(LongId<AuthUserId>(), Rnd.DateTime, Rnd.DateTime);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.ExecuteAsync(Arg.Any<Expression<Func<JourneyEntity, int?>>>())
			.Returns(Create.None<int?>());

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Fluent.AssertCalls(
			_ => { },
			_ => { },
			_ => { },
			c => FluentQueryHelper.AssertSort<JourneyEntity, DateTime>(c, x => x.Day, SortOrder.Ascending),
			c => FluentQueryHelper.AssertSort<JourneyEntity, int>(c, x => x.StartMiles, SortOrder.Ascending),
			_ => { }
		);
	}
}
