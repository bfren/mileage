// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Linq.Expressions;
using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetRecentJourneys.GetRecentJourneysHandler_Tests;

public class HandleAsync_Tests : Abstracts.TestHandler
{
	private class Setup : Setup<IJourneyRepository, JourneyEntity, JourneyId, GetRecentJourneysHandler>
	{
		internal override GetRecentJourneysHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);
	}

	private (GetRecentJourneysHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task Logs_To_Vrb__With_Query()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var query = new GetRecentJourneysQuery(userId);

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Log.Received().Vrb("Getting recent journeys for user {UserId}.", userId.Value);
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var query = new GetRecentJourneysQuery(userId);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.ExecuteAsync(Arg.Any<Expression<Func<JourneyEntity, int?>>>())
			.Returns(Create.None<int?>());

		// Act
		await handler.HandleAsync(query);

		// Assert
		var calls = v.Fluent.ReceivedCalls();
		Assert.Collection(calls,
			c => Helpers.AssertWhere<JourneyEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
			c => Helpers.AssertWhere<JourneyEntity, int?>(c, x => x.EndMiles, Compare.MoreThan, 0),
			_ => { },
			_ => { },
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
		var query = new GetRecentJourneysQuery(LongId<AuthUserId>());

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.ExecuteAsync(Arg.Any<Expression<Func<JourneyEntity, int?>>>())
			.Returns(Create.None<int?>());

		// Act
		await handler.HandleAsync(query);

		// Assert
		var calls = v.Fluent.ReceivedCalls();
		Assert.Collection(calls,
			_ => { },
			_ => { },
			c => Helpers.AssertSort<JourneyEntity, DateTime>(c, x => x.Day, SortOrder.Descending),
			c => Helpers.AssertSort<JourneyEntity, CarId>(c, x => x.CarId, SortOrder.Ascending),
			c => Helpers.AssertSort<JourneyEntity, int>(c, x => x.StartMiles, SortOrder.Descending),
			_ => { },
			_ => { }
		);
	}

	[Fact]
	public async Task Calls_FluentQuery_Maximum__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new GetRecentJourneysQuery(LongId<AuthUserId>());

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.ExecuteAsync(Arg.Any<Expression<Func<JourneyEntity, int?>>>())
			.Returns(Create.None<int?>());

		// Act
		await handler.HandleAsync(query);

		// Assert
		var calls = v.Fluent.ReceivedCalls();
		Assert.Collection(calls,
			_ => { },
			_ => { },
			_ => { },
			_ => { },
			_ => { },
			c => Helpers.AssertMaximum(c, 5),
			_ => { }
		);
	}
}
