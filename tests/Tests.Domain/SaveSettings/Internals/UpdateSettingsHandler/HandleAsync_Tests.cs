// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveSettings.Internals.UpdateSettingsHandler_Tests;

public class HandleAsync_Tests : Abstracts.TestHandler
{
	private class Setup : Setup<ISettingsRepository, SettingsEntity, SettingsId, UpdateSettingsHandler>
	{
		internal override UpdateSettingsHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);
	}

	private (UpdateSettingsHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task Calls_Logs_Vrb__With_UserId()
	{
		// Arrange
		var (handler, v) = GetVars();
		var settingsId = LongId<SettingsId>();
		var userId = LongId<AuthUserId>();
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
		var settingsId = LongId<SettingsId>();
		var version = Rnd.Lng;
		var userId = LongId<AuthUserId>();
		var carId = LongId<CarId>();
		var placeId = LongId<PlaceId>();
		var existingSettings = new SettingsEntity
		{
			Id = settingsId,
			Version = Rnd.Lng,
			UserId = userId,
			DefaultCarId = LongId<CarId>(),
			DefaultFromPlaceId = LongId<PlaceId>()
		};
		var updatedSettings = new Settings
		{
			Version = version,
			DefaultCarId = carId,
			DefaultFromPlaceId = placeId
		};
		var command = new UpdateSettingsCommand(existingSettings, updatedSettings);
		v.Repo.UpdateAsync<SettingsEntity>(default!)
			.ReturnsForAnyArgs(false);

		// Act
		await handler.HandleAsync(command);

		// Assert
		await v.Repo.Received().UpdateAsync(Arg.Is<SettingsEntity>(x =>
			x.Id == settingsId
			&& x.Version == version
			&& x.UserId == userId
			&& x.DefaultCarId == carId
			&& x.DefaultFromPlaceId == placeId
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
		var some = result.AssertSome();
		Assert.Equal(value, some);
	}
}
