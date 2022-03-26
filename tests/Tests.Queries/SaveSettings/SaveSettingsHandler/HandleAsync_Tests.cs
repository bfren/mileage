// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using MaybeF;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;
using Mileage.Queries.CheckCarBelongsToUser;
using Mileage.Queries.CheckPlaceBelongsToUser;
using Mileage.Queries.SaveSettings.Messages;

namespace Mileage.Queries.SaveSettings.SaveSettingsHandler_Tests;

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
		var settings = new Settings(null, Rnd.Lng, carId, null);
		var query = new SaveSettingsCommand(userId, settings);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(F.True.AsTask);
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
		var settings = new Settings(null, Rnd.Lng, null, placeId);
		var query = new SaveSettingsCommand(userId, settings);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(F.True.AsTask);
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
		var settings = new Settings(null, Rnd.Lng, new(Rnd.Lng), null);
		var query = new SaveSettingsCommand(new(Rnd.Lng), settings);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(F.False.AsTask);

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
		var settings = new Settings(null, Rnd.Lng, null, new(Rnd.Lng));
		var query = new SaveSettingsCommand(new(Rnd.Lng), settings);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(F.False.AsTask);

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
		var settings = new Settings(null, Rnd.Lng, null, new(Rnd.Lng));
		var query = new SaveSettingsCommand(new(Rnd.Lng), settings);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(F.True.AsTask);

		// Act

		// Assert
	}
}
