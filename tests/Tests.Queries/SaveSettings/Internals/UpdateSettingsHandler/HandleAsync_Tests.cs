// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Queries.SaveSettings.Internals.UpdateSettingsHandler_Tests;

public class HandleAsync_Tests : TestHandler<ISettingsRepository, SettingsEntity, SettingsId, UpdateSettingsHandler>
{
	public override UpdateSettingsHandler GetHandler(Vars v) =>
		new(v.Repo, v.Log);

	[Fact]
	public async Task Calls_Logs_Vrb__With_UserId()
	{
		// Arrange
		var (handler, v) = GetVars();
		var settingsId = new SettingsId(Rnd.Lng);
		var userId = new AuthUserId(Rnd.Lng);
		var command = new UpdateSettingsCommand(new() { Id = settingsId, UserId = userId }, new());
		v.Repo.UpdateAsync<SettingsEntity>(default!)
			.ReturnsForAnyArgs(false);

		// Act
		await handler.HandleAsync(command);

		// Assert
		v.Log.Received().Vrb("Updating settings {SettingsId} for user {UserId}.", settingsId.Value, userId.Value);
	}

	[Fact]
	public async Task Calls_Repo_CreateAsync__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var settingsId = new SettingsId(Rnd.Lng);
		var version = Rnd.Lng;
		var userId = new AuthUserId(Rnd.Lng);
		var carId = new CarId(Rnd.Lng);
		var placeId = new PlaceId(Rnd.Lng);
		var existingSettings = new SettingsEntity
		{
			Id = settingsId,
			Version = version,
			UserId = userId,
			DefaultCarId = new(Rnd.Lng),
			DefaultFromPlaceId = new(Rnd.Lng)
		};
		var updatedSettings = new Settings
		{
			DefaultCarId = carId,
			DefaultFromPlaceId = placeId
		};
		var command = new UpdateSettingsCommand(existingSettings, updatedSettings);
		v.Repo.UpdateAsync<SettingsEntity>(default!)
			.ReturnsForAnyArgs(false);

		// Act
		await handler.HandleAsync(command);

		// Assert
		await v.Repo.Received().UpdateAsync(Arg.Is<SettingsEntity>(
			x => x.Id == settingsId && x.Version == version && x.UserId == userId && x.DefaultCarId == carId && x.DefaultFromPlaceId == placeId
		));
	}

	[Fact]
	public async Task Calls_Repo_CreateAsync__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var value = Rnd.Flip;
		var command = new UpdateSettingsCommand(new(), new());
		v.Repo.UpdateAsync<SettingsEntity>(default!)
			.ReturnsForAnyArgs(value);

		// Act
		var result = await handler.HandleAsync(command);

		// Assert
		result.AssertTrue();
	}
}
