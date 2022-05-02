// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Jeebs.Data.Testing.Query;
using Mileage.Domain.CheckCarBelongsToUser;
using Mileage.Domain.CheckPlacesBelongToUser;
using Mileage.Domain.CheckRateBelongsToUser;
using Mileage.Domain.SaveSettings.Internals;
using Mileage.Domain.SaveSettings.Messages;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveSettings.SaveSettingsHandler_Tests;

public class HandleAsync_Tests : Abstracts.TestHandler
{
	private class Setup : Setup<ISettingsRepository, SettingsEntity, SettingsId, SaveSettingsHandler>
	{
		internal override SaveSettingsHandler GetHandler(Vars v) =>
			new(v.Dispatcher, v.Repo, v.Log);
	}

	private (SaveSettingsHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task Checks_Car_Belongs_To_User_With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var carId = LongId<CarId>();
		var settings = new Settings(LongId<SettingsId>(), Rnd.Lng, carId, null, null);
		var query = new SaveSettingsCommand(userId, settings);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<SettingsEntity>()
			.Returns(new SettingsEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<CheckCarBelongsToUserQuery>(x => x.UserId == userId && x.CarId == carId)
		);
	}

	[Fact]
	public async Task Checks_Place_Belongs_To_User_With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var placeId = LongId<PlaceId>();
		var settings = new Settings(LongId<SettingsId>(), Rnd.Lng, null, placeId, null);
		var query = new SaveSettingsCommand(userId, settings);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<SettingsEntity>()
			.Returns(new SettingsEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<CheckPlacesBelongToUserQuery>(x => x.UserId == userId && x.PlaceIds[0] == placeId)
		);
	}

	[Fact]
	public async Task Checks_Rate_Belongs_To_User_With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var rateId = LongId<RateId>();
		var settings = new Settings(LongId<SettingsId>(), Rnd.Lng, null, null, rateId);
		var query = new SaveSettingsCommand(userId, settings);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<SettingsEntity>()
			.Returns(new SettingsEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<CheckRateBelongsToUserQuery>(x => x.UserId == userId && x.RateId == rateId)
		);
	}

	[Fact]
	public async Task Car_Does_Not_Belong_To_User__Returns_None_With_SaveSettingsCheckFailedMsg()
	{
		// Arrange
		var (handler, v) = GetVars();
		var settings = new Settings(LongId<SettingsId>(), Rnd.Lng, LongId<CarId>(), null, null);
		var query = new SaveSettingsCommand(LongId<AuthUserId>(), settings);

		v.Dispatcher.DispatchAsync(Arg.Any<CheckCarBelongsToUserQuery>())
			.ReturnsForAnyArgs(false);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var msg = result.AssertNone();
		Assert.IsType<SaveSettingsCheckFailedMsg>(msg);
	}

	[Fact]
	public async Task Place_Does_Not_Belong_To_User__Returns_None_With_SaveSettingsCheckFailedMsg()
	{
		// Arrange
		var (handler, v) = GetVars();
		var settings = new Settings(LongId<SettingsId>(), Rnd.Lng, null, LongId<PlaceId>(), null);
		var query = new SaveSettingsCommand(LongId<AuthUserId>(), settings);

		v.Dispatcher.DispatchAsync(Arg.Any<CheckPlacesBelongToUserQuery>())
			.ReturnsForAnyArgs(false);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var msg = result.AssertNone();
		Assert.IsType<SaveSettingsCheckFailedMsg>(msg);
	}

	[Fact]
	public async Task Rate_Does_Not_Belong_To_User__Returns_None_With_SaveSettingsCheckFailedMsg()
	{
		// Arrange
		var (handler, v) = GetVars();
		var settings = new Settings(LongId<SettingsId>(), Rnd.Lng, null, null, LongId<RateId>());
		var query = new SaveSettingsCommand(LongId<AuthUserId>(), settings);

		v.Dispatcher.DispatchAsync(Arg.Any<CheckRateBelongsToUserQuery>())
			.ReturnsForAnyArgs(false);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var msg = result.AssertNone();
		Assert.IsType<SaveSettingsCheckFailedMsg>(msg);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_Where__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var settings = new Settings(LongId<SettingsId>(), Rnd.Lng, null, null, null);
		var query = new SaveSettingsCommand(userId, settings);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<SettingsEntity>()
			.Returns(new SettingsEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Fluent.AssertCalls(
			c => FluentQueryHelper.AssertWhere<SettingsEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
			_ => { }
		);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_Some__Dispatches_Update__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var settings = new Settings(LongId<SettingsId>(), Rnd.Lng, null, null, null);
		var existingSettings = new SettingsEntity();
		var query = new SaveSettingsCommand(LongId<AuthUserId>(), settings);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<SettingsEntity>()
			.Returns(existingSettings);

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<UpdateSettingsCommand>(
				x => x.ExistingSettings == existingSettings && x.UpdatedSettings == settings
			)
		);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_Some__Dispatches_Update__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var settings = new Settings(LongId<SettingsId>(), Rnd.Lng, null, null, null);
		var query = new SaveSettingsCommand(LongId<AuthUserId>(), settings);
		var updated = Rnd.Flip;

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.DispatchAsync(Arg.Any<UpdateSettingsCommand>())
			.Returns(updated);
		v.Fluent.QuerySingleAsync<SettingsEntity>()
			.Returns(new SettingsEntity());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(Arg.Any<UpdateSettingsCommand>());
		var some = result.AssertSome();
		Assert.Equal(updated, some);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_None__Dispatches_Create__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var settings = new Settings(LongId<SettingsId>(), Rnd.Lng, null, null, null);
		var query = new SaveSettingsCommand(userId, settings);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<SettingsEntity>()
			.Returns(Create.None<SettingsEntity>());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<CreateSettingsCommand>(
				x => x.UserId == userId && x.Settings == settings
			)
		);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_None__Dispatches_Create__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var settings = new Settings(LongId<SettingsId>(), Rnd.Lng, null, null, null);
		var query = new SaveSettingsCommand(LongId<AuthUserId>(), settings);
		var updated = Rnd.Flip;

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.DispatchAsync(Arg.Any<CreateSettingsCommand>())
			.Returns(updated);
		v.Fluent.QuerySingleAsync<SettingsEntity>()
			.Returns(Create.None<SettingsEntity>());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(Arg.Any<CreateSettingsCommand>());
		var some = result.AssertSome();
		Assert.Equal(updated, some);
	}
}
