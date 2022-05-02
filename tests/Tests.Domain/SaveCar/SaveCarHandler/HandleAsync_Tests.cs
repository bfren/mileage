// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Jeebs.Data.Testing.Query;
using Mileage.Domain.CheckCarBelongsToUser;
using Mileage.Domain.SaveCar.Internals;
using Mileage.Domain.SaveCar.Messages;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveCar.SaveCarHandler_Tests;

public class HandleAsync_Tests : Abstracts.TestHandler
{
	private class Setup : Setup<ICarRepository, CarEntity, CarId, SaveCarHandler>
	{
		internal override SaveCarHandler GetHandler(Vars v) =>
			new(v.Dispatcher, v.Repo, v.Log);
	}

	private (SaveCarHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task Calls_Log_Vrb__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new SaveCarQuery();
		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<CarEntity>()
			.Returns(new CarEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Log.Received().Vrb("Saving Car {Query}.", query);
	}

	[Fact]
	public async Task Checks_Car_Belongs_To_User_With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var carId = LongId<CarId>();
		var query = new SaveCarQuery(userId, carId, Rnd.Lng, Rnd.Str, Rnd.Str, Rnd.Flip);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<CarEntity>()
			.Returns(new CarEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<CheckCarBelongsToUserQuery>(x => x.UserId == userId && x.CarId == carId)
		);
	}

	[Fact]
	public async Task Car_Does_Not_Belong_To_User__Returns_None_With_CarDoesNotBelongToUserMsg()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new SaveCarQuery(LongId<AuthUserId>(), LongId<CarId>(), Rnd.Lng, Rnd.Str, Rnd.Str, Rnd.Flip);

		v.Dispatcher.DispatchAsync(Arg.Any<CheckCarBelongsToUserQuery>())
			.ReturnsForAnyArgs(false);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		result.AssertNone().AssertType<CarDoesNotBelongToUserMsg>();
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_Where__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var carId = LongId<CarId>();
		var query = new SaveCarQuery(userId, carId, Rnd.Lng, Rnd.Str, Rnd.Str, Rnd.Flip);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<CarEntity>()
			.Returns(new CarEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Fluent.AssertCalls(
			c => FluentQueryHelper.AssertWhere<CarEntity, CarId>(c, x => x.Id, Compare.Equal, carId),
			c => FluentQueryHelper.AssertWhere<CarEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
			_ => { }
		);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_Some__Dispatches_Update__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var carId = LongId<CarId>();
		var version = Rnd.Lng;
		var description = Rnd.Str;
		var plate = Rnd.Str;
		var disabled = Rnd.Flip;
		var query = new SaveCarQuery(userId, carId, version, description, plate, disabled);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<CarEntity>()
			.Returns(new CarEntity { Id = carId });

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<UpdateCarCommand>(c =>
				c.Id == carId
				&& c.Version == version
				&& c.Description == description
				&& c.NumberPlate == plate
				&& c.IsDisabled == disabled
			)
		);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_Some__Dispatches_Update__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var carId = LongId<CarId>();
		var query = new SaveCarQuery(LongId<AuthUserId>(), LongId<CarId>(), Rnd.Lng, Rnd.Str, Rnd.Str, Rnd.Flip);
		var updated = Rnd.Flip;

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.DispatchAsync(Arg.Any<UpdateCarCommand>())
			.Returns(updated);
		v.Fluent.QuerySingleAsync<CarEntity>()
			.Returns(new CarEntity { Id = carId });

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(Arg.Any<UpdateCarCommand>());
		var some = result.AssertSome();
		Assert.Equal(carId, some);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_None__Dispatches_Create__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var description = Rnd.Str;
		var plate = Rnd.Str;
		var query = new SaveCarQuery(userId, null, 0L, description, plate, Rnd.Flip);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<CarEntity>()
			.Returns(Create.None<CarEntity>());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<CreateCarQuery>(c =>
				c.UserId == userId
				&& c.Description == description
				&& c.NumberPlate == plate
			)
		);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_None__Dispatches_Create__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var carId = LongId<CarId>();
		var query = new SaveCarQuery(LongId<AuthUserId>(), LongId<CarId>(), Rnd.Lng, Rnd.Str, Rnd.Str, Rnd.Flip);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.DispatchAsync(Arg.Any<CreateCarQuery>())
			.Returns(carId);
		v.Fluent.QuerySingleAsync<CarEntity>()
			.Returns(Create.None<CarEntity>());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(Arg.Any<CreateCarQuery>());
		var some = result.AssertSome();
		Assert.Equal(carId, some);
	}
}
