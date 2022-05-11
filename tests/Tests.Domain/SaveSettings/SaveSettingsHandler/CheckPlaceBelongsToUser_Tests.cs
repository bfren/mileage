// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveSettings.SaveSettingsHandler_Tests;

public class CheckPlaceBelongsToUser_Tests : Abstracts.TestHandler
{
	private class Setup : Setup<ISettingsRepository, SettingsEntity, SettingsId, SaveSettingsHandler>
	{
		internal override SaveSettingsHandler GetHandler(Vars v) =>
			new(v.Dispatcher, v.Repo, v.Log);
	}

	private (SaveSettingsHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task With_PlaceId__Calls_Dispatcher_DispatchAsync__Receives_Some__Returns_Value()
	{
		// Arrange
		var (handler, v) = GetVars();
		var value = Rnd.Flip;
		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(F.Some(value).AsTask());

		// Act
		var result = await handler.CheckPlaceBelongsToUser(LongId<PlaceId>(), LongId<AuthUserId>());

		// Assert
		Assert.Equal(value, result);
	}

	[Fact]
	public async Task With_PlaceId__Calls_Dispatcher_DispatchAsync__Receives_None__Returns_False()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(Create.None<bool>());

		// Act
		var result = await handler.CheckPlaceBelongsToUser(LongId<PlaceId>(), LongId<AuthUserId>());

		// Assert
		Assert.False(result);
	}

	[Fact]
	public async Task Without_PlaceId__Returns_True()
	{
		// Arrange
		var (handler, _) = GetVars();

		// Act
		var result = await handler.CheckPlaceBelongsToUser(null, new());

		// Assert
		Assert.True(result);
	}
}
