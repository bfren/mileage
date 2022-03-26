// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Messages;
using MaybeF;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveSettings.Internals.CreateSettingsHandler_Tests;

public class HandleAsync_Tests : TestHandler<ISettingsRepository, SettingsEntity, SettingsId, CreateSettingsHandler>
{
	public override CreateSettingsHandler GetHandler(Vars v) =>
		new(v.Repo, v.Log);

	[Fact]
	public async Task Calls_Logs_Vrb__With_UserId()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = new AuthUserId(Rnd.Lng);
		var command = new CreateSettingsCommand(userId, new());
		v.Repo.CreateAsync(default!)
			.ReturnsForAnyArgs(Create.None<SettingsId>());

		// Act
		await handler.HandleAsync(command);

		// Assert
		v.Log.Received().Vrb("Creating new settings for user {UserId}.", userId.Value);
	}

	[Fact]
	public async Task Calls_Repo_CreateAsync__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = new AuthUserId(Rnd.Lng);
		var settingsId = new SettingsId(Rnd.Lng);
		var carId = new CarId(Rnd.Lng);
		var placeId = new PlaceId(Rnd.Lng);
		var command = new CreateSettingsCommand(userId, new(0L, carId, placeId));
		v.Repo.CreateAsync(default!)
			.ReturnsForAnyArgs(settingsId);

		// Act
		await handler.HandleAsync(command);

		// Assert
		await v.Repo.Received().CreateAsync(Arg.Is<SettingsEntity>(
			x => x.UserId == userId && x.DefaultCarId == carId && x.DefaultFromPlaceId == placeId
		));
	}

	[Fact]
	public async Task Calls_Repo_CreateAsync__Receives_Some__Audits_To_Vrb()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = new AuthUserId(Rnd.Lng);
		var settingsId = new SettingsId(Rnd.Lng);
		var command = new CreateSettingsCommand(userId, new());
		v.Repo.CreateAsync(default!)
			.ReturnsForAnyArgs(settingsId);

		// Act
		await handler.HandleAsync(command);

		// Assert
		v.Log.Received().Vrb("Created settings {SettingsId} for user {UserId}.", settingsId.Value, userId.Value);
	}

	[Fact]
	public async Task Calls_Repo_CreateAsync__Receives_Some__Returns_True()
	{
		// Arrange
		var (handler, v) = GetVars();
		var command = new CreateSettingsCommand(default, new());
		v.Repo.CreateAsync(default!)
			.ReturnsForAnyArgs(new SettingsId(Rnd.Lng));

		// Act
		var result = await handler.HandleAsync(command);

		// Assert
		result.AssertTrue();
	}

	[Fact]
	public async void Calls_Repo_CreateAsync__Receives_None__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var command = new CreateSettingsCommand(default, new());
		var msg = new TestMsg();
		var failed = F.None<SettingsId>(msg);
		v.Repo.CreateAsync(default!)
			.ReturnsForAnyArgs(failed);

		// Act
		var result = await handler.HandleAsync(command);

		// Assert
		var none = result.AssertNone();
		Assert.Same(msg, none);
	}

	public sealed record class TestMsg : Msg;
}
