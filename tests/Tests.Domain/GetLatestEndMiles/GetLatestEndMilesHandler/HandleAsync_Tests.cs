// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Linq.Expressions;
using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;
using static MaybeF.F.M;

namespace Mileage.Domain.GetLatestEndMiles.GetLatestEndMilesHandler_Tests;

public class HandleAsync_Tests : Abstracts.TestHandler
{
	private class Setup : Setup<IJourneyRepository, JourneyEntity, JourneyId, GetLatestEndMilesHandler>
	{
		internal override GetLatestEndMilesHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);
	}

	private (GetLatestEndMilesHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task Logs_To_Vrb__With_Query()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var carId = LongId<CarId>();
		var query = new GetLatestEndMilesQuery(userId, carId);

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Log.Received().Vrb("Getting latest end miles for User {UserId} and Car {CarId}.", userId.Value, carId.Value);
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var carId = LongId<CarId>();
		var query = new GetLatestEndMilesQuery(userId, carId);

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
			c => Helpers.AssertWhere<JourneyEntity, CarId>(c, x => x.CarId, Compare.Equal, carId),
			_ => { },
			_ => { }
		);
	}

	[Fact]
	public async Task Calls_FluentQuery_Sort__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new GetLatestEndMilesQuery(LongId<AuthUserId>(), LongId<CarId>());

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
			c => Helpers.AssertSort<JourneyEntity, int>(c, x => x.StartMiles, SortOrder.Descending),
			_ => { }
		);
	}

	[Fact]
	public async Task Calls_FluentQuery_ExecuteAsync__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new GetLatestEndMilesQuery(LongId<AuthUserId>(), LongId<CarId>());

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
			c => Helpers.AssertExecute<JourneyEntity, int?>(c, x => x.EndMiles)
		);
	}

	[Fact]
	public async Task Calls_FluentQuery_ExecuteAsync__Receives_None__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new GetLatestEndMilesQuery(LongId<AuthUserId>(), LongId<CarId>());
		var msg = Substitute.For<IMsg>();
		var maybe = F.None<int?>(msg);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.ExecuteAsync(Arg.Any<Expression<Func<JourneyEntity, int?>>>())
			.Returns(maybe);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var none = result.AssertNone();
		Assert.Same(msg, none);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public async Task Calls_FluentQuery_ExecuteAsync__Receives_Null__Returns_Zero(bool asNone)
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new GetLatestEndMilesQuery(LongId<AuthUserId>(), LongId<CarId>());

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.ExecuteAsync(Arg.Any<Expression<Func<JourneyEntity, int?>>>())
			.Returns(asNone switch
			{
				true =>
					F.None<int?, NullValueMsg>(),

				false =>
					F.Some<int?>(null, true)
			});

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var some = result.AssertSome();
		Assert.Equal(0u, some);
	}

	[Fact]
	public async Task Calls_FluentQuery_ExecuteAsync__Receives_Some__Returns_Value()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new GetLatestEndMilesQuery(LongId<AuthUserId>(), LongId<CarId>());
		var value = Rnd.UInt;

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.ExecuteAsync(Arg.Any<Expression<Func<JourneyEntity, int?>>>())
			.Returns((int)value);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var some = result.AssertSome();
		Assert.Equal(value, some);
	}
}
