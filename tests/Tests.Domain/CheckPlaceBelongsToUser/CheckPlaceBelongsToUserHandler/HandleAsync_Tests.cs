// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Jeebs.Messages;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.CheckPlaceBelongsToUser.CheckPlaceBelongsToUserHandler_Tests;

public class HandleAsync_Tests : TestHandler
{
	private class Setup : Setup<IPlaceRepository, PlaceEntity, PlaceId, CheckPlaceBelongsToUserHandler>
	{
		internal override CheckPlaceBelongsToUserHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);
	}

	private (CheckPlaceBelongsToUserHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task Calls_Log_Vrb__With_Query()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QuerySingleAsync<PlaceEntity>()
			.Returns(new PlaceEntity());
		var query = new CheckPlaceBelongsToUserQuery(RndId<AuthUserId>(), RndId<PlaceId>());

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Log.Received().Vrb("Checking place {PlaceId} belongs to user {UserId}.", query.PlaceId.Value, query.UserId.Value);
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QuerySingleAsync<PlaceEntity>()
			.Returns(new PlaceEntity());
		var carId = RndId<PlaceId>();
		var userId = RndId<AuthUserId>();
		var query = new CheckPlaceBelongsToUserQuery(userId, carId);

		// Act
		await handler.HandleAsync(query);

		// Assert
		var calls = v.Fluent.ReceivedCalls();
		Assert.Collection(calls,
			c => Helpers.AssertWhere<PlaceEntity, PlaceId>(c, x => x.Id, Compare.Equal, carId),
			c => Helpers.AssertWhere<PlaceEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
			_ => { }
		);
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__Receives_None__Calls_Log_Msg()
	{
		// Arrange
		var (handler, v) = GetVars();
		var msg = new TestMsg();
		v.Fluent.QuerySingleAsync<PlaceEntity>()
			.Returns(F.None<PlaceEntity>(msg));
		var query = new CheckPlaceBelongsToUserQuery(new(), new());

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Log.Received().Msg(msg);
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__Receives_None__Returns_False()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QuerySingleAsync<PlaceEntity>()
				.Returns(Create.None<PlaceEntity>());
		var query = new CheckPlaceBelongsToUserQuery(new(), new());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		result.AssertFalse();
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__Receives_Some__Returns_True()
	{
		// Arrange
		var (handler, v) = GetVars();
		var entity = new PlaceEntity();
		v.Fluent.QuerySingleAsync<PlaceEntity>()
			.Returns(entity);
		var query = new CheckPlaceBelongsToUserQuery(new(), new());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		result.AssertTrue();
	}

	public sealed record class TestMsg : Msg;
}
