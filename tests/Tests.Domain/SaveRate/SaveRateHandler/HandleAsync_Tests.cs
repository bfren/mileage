// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Mileage.Domain.CheckRateBelongsToUser;
using Mileage.Domain.SaveRate.Internals;
using Mileage.Domain.SaveRate.Messages;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveRate.SaveRateHandler_Tests;

public class HandleAsync_Tests : Abstracts.TestHandler
{
	private class Setup : Setup<IRateRepository, RateEntity, RateId, SaveRateHandler>
	{
		internal override SaveRateHandler GetHandler(Vars v) =>
			new(v.Dispatcher, v.Repo, v.Log);
	}

	private (SaveRateHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task Checks_Rate_Belongs_To_User_With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var rateId = LongId<RateId>();
		var query = new SaveRateQuery(userId, rateId, Rnd.Lng, Rnd.Flt);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<RateEntity>()
			.Returns(new RateEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<CheckRateBelongsToUserQuery>(x => x.UserId == userId && x.RateId == rateId)
		);
	}

	[Fact]
	public async Task Rate_Does_Not_Belong_To_User__Returns_None_With_RateDoesNotBelongToUserMsg()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new SaveRateQuery(LongId<AuthUserId>(), LongId<RateId>(), Rnd.Lng, Rnd.Flt);

		v.Dispatcher.DispatchAsync(Arg.Any<CheckRateBelongsToUserQuery>())
			.ReturnsForAnyArgs(false);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		result.AssertNone().AssertType<RateDoesNotBelongToUserMsg>();
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_Where__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var carId = LongId<RateId>();
		var query = new SaveRateQuery(userId, carId, Rnd.Lng, Rnd.Flt);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<RateEntity>()
			.Returns(new RateEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		var calls = v.Fluent.ReceivedCalls();
		Assert.Collection(calls,
			c => Helpers.AssertWhere<RateEntity, RateId>(c, x => x.Id, Compare.Equal, carId),
			c => Helpers.AssertWhere<RateEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
			_ => { }
		);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_Some__Dispatches_Update__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var rateId = LongId<RateId>();
		var version = Rnd.Lng;
		var amountPerMileInGBP = Rnd.Flt;
		var query = new SaveRateQuery(userId, rateId, version, amountPerMileInGBP);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<RateEntity>()
			.Returns(new RateEntity { Id = rateId });

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<UpdateRateCommand>(c =>
				c.Id == rateId
				&& c.Version == version
				&& c.AmountPerMileGBP == amountPerMileInGBP
			)
		);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_Some__Dispatches_Update__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var rateId = LongId<RateId>();
		var query = new SaveRateQuery(LongId<AuthUserId>(), LongId<RateId>(), Rnd.Lng, Rnd.Flt);
		var updated = Rnd.Flip;

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.DispatchAsync(Arg.Any<UpdateRateCommand>())
			.Returns(updated);
		v.Fluent.QuerySingleAsync<RateEntity>()
			.Returns(new RateEntity { Id = rateId });

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(Arg.Any<UpdateRateCommand>());
		var some = result.AssertSome();
		Assert.Equal(rateId, some);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_None__Dispatches_Create__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var amountPerMileInGBP = Rnd.Flt;
		var query = new SaveRateQuery(userId, null, 0L, amountPerMileInGBP);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<RateEntity>()
			.Returns(Create.None<RateEntity>());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<CreateRateQuery>(c =>
				c.UserId == userId
				&& c.AmountPerMileGBP == amountPerMileInGBP
			)
		);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_None__Dispatches_Create__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var carId = LongId<RateId>();
		var query = new SaveRateQuery(LongId<AuthUserId>(), LongId<RateId>(), Rnd.Lng, Rnd.Flt);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.DispatchAsync(Arg.Any<CreateRateQuery>())
			.Returns(carId);
		v.Fluent.QuerySingleAsync<RateEntity>()
			.Returns(Create.None<RateEntity>());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(Arg.Any<CreateRateQuery>());
		var some = result.AssertSome();
		Assert.Equal(carId, some);
	}
}
