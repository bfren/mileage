// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Linq.Expressions;
using Jeebs.Auth.Data.Ids;
using Jeebs.Data.Enums;
using Jeebs.Data.Testing.Query;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetLatestEndMiles.GetLatestEndMilesHandler_Tests;

public sealed class HandleAsync_Tests : Abstracts.TestHandler
{
	private sealed class Setup : Setup<IJourneyRepository, JourneyEntity, JourneyId, GetLatestEndMilesHandler>
	{
		internal override GetLatestEndMilesHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);
	}

	private (GetLatestEndMilesHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task CarId_Null__Returns_Zero()
	{
		// Arrange
		var (handler, _) = GetVars();
		var userId = IdGen.LongId<AuthUserId>();
		var query = new GetLatestEndMilesQuery(userId, null);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		result.AssertOk(0u);
	}

	[Fact]
	public async Task Logs_To_Vrb__With_Query()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = IdGen.LongId<AuthUserId>();
		var carId = IdGen.LongId<CarId>();
		var query = new GetLatestEndMilesQuery(userId, carId);
		v.Fluent.ExecuteAsync(Arg.Any<Expression<Func<JourneyEntity, int?>>>())
			.Returns(FailGen.Create<int?>());

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
		var userId = IdGen.LongId<AuthUserId>();
		var carId = IdGen.LongId<CarId>();
		var query = new GetLatestEndMilesQuery(userId, carId);

		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.ExecuteAsync(Arg.Any<Expression<Func<JourneyEntity, int?>>>())
			.Returns(FailGen.Create<int?>());

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Fluent.AssertCalls(
			c => FluentQueryHelper.AssertWhere<JourneyEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
			c => FluentQueryHelper.AssertWhere<JourneyEntity, CarId>(c, x => x.CarId, Compare.Equal, carId),
			_ => { },
			_ => { }
		);
	}

	[Fact]
	public async Task Calls_FluentQuery_Sort__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new GetLatestEndMilesQuery(IdGen.LongId<AuthUserId>(), IdGen.LongId<CarId>());

		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.ExecuteAsync(Arg.Any<Expression<Func<JourneyEntity, int?>>>())
			.Returns(FailGen.Create<int?>());

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Fluent.AssertCalls(
			_ => { },
			_ => { },
			c => FluentQueryHelper.AssertSort<JourneyEntity, int>(c, x => x.StartMiles, SortOrder.Descending),
			_ => { }
		);
	}

	[Fact]
	public async Task Calls_FluentQuery_ExecuteAsync__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new GetLatestEndMilesQuery(IdGen.LongId<AuthUserId>(), IdGen.LongId<CarId>());

		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.ExecuteAsync(Arg.Any<Expression<Func<JourneyEntity, int?>>>())
			.Returns(FailGen.Create<int?>());

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Fluent.AssertCalls(
			_ => { },
			_ => { },
			_ => { },
			c => FluentQueryHelper.AssertExecute<JourneyEntity, int?>(c, x => x.EndMiles)
		);
	}

	[Fact]
	public async Task Calls_FluentQuery_ExecuteAsync__Receives_None__Returns_Zero()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new GetLatestEndMilesQuery(IdGen.LongId<AuthUserId>(), IdGen.LongId<CarId>());
		var failure = FailGen.Create<int?>();

		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.ExecuteAsync(Arg.Any<Expression<Func<JourneyEntity, int?>>>())
			.Returns(failure);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		result.AssertOk(0u);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public async Task Calls_FluentQuery_ExecuteAsync__Receives_Null__Returns_Zero(bool asNone)
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new GetLatestEndMilesQuery(IdGen.LongId<AuthUserId>(), IdGen.LongId<CarId>());

		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.ExecuteAsync(Arg.Any<Expression<Func<JourneyEntity, int?>>>())
			.Returns(asNone switch
			{
				true =>
					FailGen.Create<int?>(),

				false =>
					R.Wrap<int?>(null)
			});

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		result.AssertOk(0u);
	}

	[Fact]
	public async Task Calls_FluentQuery_ExecuteAsync__Receives_Some__Returns_Value()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new GetLatestEndMilesQuery(IdGen.LongId<AuthUserId>(), IdGen.LongId<CarId>());
		var value = Rnd.UInt32;

		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.ExecuteAsync(Arg.Any<Expression<Func<JourneyEntity, int?>>>())
			.Returns((int)value);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		result.AssertOk(value);
	}
}
