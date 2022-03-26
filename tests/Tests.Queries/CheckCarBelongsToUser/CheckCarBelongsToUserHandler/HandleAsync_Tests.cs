// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Jeebs.Messages;
using MaybeF;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Queries.CheckCarBelongsToUser.CheckCarBelongsToUserHandler_Tests;

public class HandleAsync_Tests : TestHandler<ICarRepository, CarEntity, CarId, CheckCarBelongsToUserHandler>
{
	public override CheckCarBelongsToUserHandler GetHandler(Vars v) =>
		new(v.Repo, v.Log);

	[Fact]
	public async Task Calls_Log_Vrb__With_Query()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QuerySingleAsync<CarEntity>()
			.Returns(new CarEntity());
		var query = new CheckCarBelongsToUserQuery(new(), new());

		// Act
		await handler.HandleAsync(query, CancellationToken.None);

		// Assert
		v.Log.Received().Vrb("Checking car {CarId} belongs to user {UserId}.", query.CarId.Value, query.UserId.Value);
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QuerySingleAsync<CarEntity>()
			.Returns(new CarEntity());
		var carId = new CarId(Rnd.Lng);
		var userId = new AuthUserId(Rnd.Lng);
		var query = new CheckCarBelongsToUserQuery(userId, carId);

		// Act
		await handler.HandleAsync(query, CancellationToken.None);

		// Assert
		var calls = v.Fluent.ReceivedCalls();
		Assert.Collection(calls,
			c => Helpers.AssertWhere<CarEntity, CarId>(c, x => x.Id, Compare.Equal, carId),
			c => Helpers.AssertWhere<CarEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
			_ => { }
		);
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__Receives_None__Audits_Msg()
	{
		// Arrange
		var (handler, v) = GetVars();
		var msg = new TestMsg();
		v.Fluent.QuerySingleAsync<CarEntity>()
			.Returns(F.None<CarEntity>(msg));
		var query = new CheckCarBelongsToUserQuery(new(), new());

		// Act
		await handler.HandleAsync(query, CancellationToken.None);

		// Assert
		v.Log.Received().Msg(msg);
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__Receives_None__Returns_False()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QuerySingleAsync<CarEntity>()
			.Returns(Create.None<CarEntity>());
		var query = new CheckCarBelongsToUserQuery(new(), new());

		// Act
		var result = await handler.HandleAsync(query, CancellationToken.None);

		// Assert
		result.AssertFalse();
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__Receives_Some__Returns_True()
	{
		// Arrange
		var (handler, v) = GetVars();
		var entity = new CarEntity();
		v.Fluent.QuerySingleAsync<CarEntity>()
			.Returns(entity);
		var query = new CheckCarBelongsToUserQuery(new(), new());

		// Act
		var result = await handler.HandleAsync(query, CancellationToken.None);

		// Assert
		result.AssertTrue();
	}

	public sealed record class TestMsg : Msg;
}
