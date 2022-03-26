// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;
using Mileage.Domain.CheckCarBelongsToUser;
using Mileage.Domain.CheckPlaceBelongsToUser;
using Mileage.Domain.SaveSettings.Internals;
using Mileage.Domain.SaveSettings.Messages;

namespace Mileage.Domain.SaveSettings.SaveSettingsHandler_Tests;

public class HandleAsync_Tests : TestHandler<ISettingsRepository, SettingsEntity, SettingsId, SaveSettingsHandler>
{
	public override SaveSettingsHandler GetHandler(Vars v) =>
		new(v.Dispatcher, v.Repo, v.Log);

	[Fact]
	public async Task Checks_Car_Belongs_To_User_With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = new AuthUserId(Rnd.Lng);
		var carId = new CarId(Rnd.Lng);
		var settings = new Settings(Rnd.Lng, carId, null);
		var query = new SaveSettingsCommand(userId, settings);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<SettingsEntity>()
			.Returns(new SettingsEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<CheckCarBelongsToUserQuery>(q => q.UserId == userId && q.CarId == carId)
		);
	}

	[Fact]
	public async Task Checks_Place_Belongs_To_User_With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = new AuthUserId(Rnd.Lng);
		var placeId = new PlaceId(Rnd.Lng);
		var settings = new Settings(Rnd.Lng, null, placeId);
		var query = new SaveSettingsCommand(userId, settings);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<SettingsEntity>()
			.Returns(new SettingsEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<CheckPlaceBelongsToUserQuery>(q => q.UserId == userId && q.PlaceId == placeId)
		);
	}

	[Fact]
	public async Task Car_Does_Not_Belong_To_User__Returns_None_With_SettingsCheckFailedMsg()
	{
		// Arrange
		var (handler, v) = GetVars();
		var settings = new Settings(Rnd.Lng, new(Rnd.Lng), null);
		var query = new SaveSettingsCommand(new(Rnd.Lng), settings);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(false);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var msg = result.AssertNone();
		Assert.IsType<SettingsCheckFailedMsg>(msg);
	}

	[Fact]
	public async Task Place_Does_Not_Belong_To_User__Returns_None_With_SettingsCheckFailedMsg()
	{
		// Arrange
		var (handler, v) = GetVars();
		var settings = new Settings(Rnd.Lng, null, new(Rnd.Lng));
		var query = new SaveSettingsCommand(new(Rnd.Lng), settings);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(false);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var msg = result.AssertNone();
		Assert.IsType<SettingsCheckFailedMsg>(msg);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_Where__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = new AuthUserId(Rnd.Lng);
		var settings = new Settings(Rnd.Lng, null, new(Rnd.Lng));
		var query = new SaveSettingsCommand(userId, settings);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<SettingsEntity>()
			.Returns(new SettingsEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		var calls = v.Fluent.ReceivedCalls();
		Assert.Collection(calls,
			c => Helpers.AssertWhere<SettingsEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
			_ => { }
		);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_Some__Dispatches_Update__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var settings = new Settings(Rnd.Lng, null, new(Rnd.Lng));
		var existingSettings = new SettingsEntity();
		var query = new SaveSettingsCommand(new(Rnd.Lng), settings);

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
		var settings = new Settings(Rnd.Lng, null, new(Rnd.Lng));
		var query = new SaveSettingsCommand(new(Rnd.Lng), settings);
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
		var userId = new AuthUserId(Rnd.Lng);
		var settings = new Settings(Rnd.Lng, null, new(Rnd.Lng));
		var query = new SaveSettingsCommand(userId, settings);
		var updated = Rnd.Flip;

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
		var settings = new Settings(Rnd.Lng, null, new(Rnd.Lng));
		var query = new SaveSettingsCommand(new(Rnd.Lng), settings);
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
