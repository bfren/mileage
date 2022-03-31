// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Jeebs.Messages;
using Mileage.Domain.DeleteJourney.Messages;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.DeleteJourney.DeleteJourneyHandler_Tests;

public class HandleAsync_Tests : TestHandler<IJourneyRepository, JourneyEntity, JourneyId, DeleteJourneyHandler>
{
	public override DeleteJourneyHandler GetHandler(Vars v) =>
		new(v.Repo, v.Log);

	[Fact]
	public async Task Calls_Log_Vrb__With_Query()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QuerySingleAsync<JourneyToDelete>()
			.Returns(new JourneyToDelete(RndId<JourneyId>(), Rnd.Lng));
		var query = new DeleteJourneyQuery(new(), new());

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Log.Received().Vrb("Delete Journey: {Query}", query);
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QuerySingleAsync<JourneyToDelete>()
			.Returns(new JourneyToDelete(RndId<JourneyId>(), Rnd.Lng));
		var journeyId = RndId<JourneyId>();
		var userId = RndId<AuthUserId>();
		var query = new DeleteJourneyQuery(journeyId, userId);

		// Act
		await handler.HandleAsync(query);

		// Assert
		var calls = v.Fluent.ReceivedCalls();
		Assert.Collection(calls,
			c => Helpers.AssertWhere<JourneyEntity, JourneyId>(c, x => x.Id, Compare.Equal, journeyId),
			c => Helpers.AssertWhere<JourneyEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
			_ => { }
		);
	}

	[Fact]
	public async Task Calls_FluentQuery_WhereSingleAsync__Receives_None__Audits_Msg()
	{
		// Arrange
		var (handler, v) = GetVars();
		var msg = new TestMsg();
		v.Fluent.QuerySingleAsync<JourneyToDelete>()
			.Returns(F.None<JourneyToDelete>(msg));
		var query = new DeleteJourneyQuery(new(), new());

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Log.Received().Msg(msg);
	}

	[Fact]
	public async Task Calls_FluentQuery_WhereSingleAsync__Receives_None__Returns_None_With_JourneyDoesNotExistMsg()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QuerySingleAsync<JourneyToDelete>()
			.Returns(Create.None<JourneyToDelete>());
		var journeyId = RndId<JourneyId>();
		var userId = RndId<AuthUserId>();
		var query = new DeleteJourneyQuery(journeyId, userId);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var none = result.AssertNone();
		var msg = Assert.IsType<JourneyDoesNotExistMsg>(none);
		Assert.Equal(journeyId, msg.JourneyId);
		Assert.Equal(userId, msg.UserId);
	}

	[Fact]
	public async Task Calls_FluentQuery_WhereSingleAsync__Receives_Some__Calls_Repo_DeleteAsync__With_Correct_Value()
	{
		// Arrange
		var (handler, v) = GetVars();
		var journeyId = RndId<JourneyId>();
		var userId = RndId<AuthUserId>();
		var query = new DeleteJourneyQuery(journeyId, userId);
		var model = new JourneyToDelete(journeyId, Rnd.Lng);
		v.Fluent.QuerySingleAsync<JourneyToDelete>()
			.Returns(model);

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Repo.Received().DeleteAsync(model);
	}

	[Fact]
	public async Task Calls_FluentQuery_WhereSingleAsync__Receives_Some__Calls_Repo_DeleteAsync__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var model = new JourneyToDelete(RndId<JourneyId>(), Rnd.Lng);
		v.Fluent.QuerySingleAsync<JourneyToDelete>()
			.Returns(model);
		var expected = Rnd.Flip;
		v.Repo.DeleteAsync<JourneyToDelete>(default!)
			.ReturnsForAnyArgs(expected);
		var journeyId = RndId<JourneyId>();
		var userId = RndId<AuthUserId>();
		var query = new DeleteJourneyQuery(journeyId, userId);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var some = result.AssertSome();
		Assert.Equal(expected, some);
	}

	public sealed record class TestMsg : Msg;
}
