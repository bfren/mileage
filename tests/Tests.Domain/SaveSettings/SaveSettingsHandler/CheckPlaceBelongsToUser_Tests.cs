// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveSettings.SaveSettingsHandler_Tests;

public class CheckPlaceBelongsToUser_Tests : Abstracts.TestHandler
{
	private sealed class Setup : Setup<ISettingsRepository, SettingsEntity, SettingsId, SaveSettingsHandler>
	{
		internal override SaveSettingsHandler GetHandler(Vars v) =>
			new(v.Dispatcher, v.Repo, v.Log);
	}

	private (SaveSettingsHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task With_PlaceId__Calls_Dispatcher_SendAsync__Receives_Some__Returns_Value()
	{
		// Arrange
		var (handler, v) = GetVars();
		var value = Rnd.Flip;
		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(R.Wrap(value).AsTask());

		// Act
		var result = await handler.CheckPlaceBelongsToUser(IdGen.LongId<PlaceId>(), IdGen.LongId<AuthUserId>());

		// Assert
		Assert.Equal(value, result);
	}

	[Fact]
	public async Task With_PlaceId__Calls_Dispatcher_SendAsync__Receives_None__Returns_False()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(FailGen.Create<bool>());

		// Act
		var result = await handler.CheckPlaceBelongsToUser(IdGen.LongId<PlaceId>(), IdGen.LongId<AuthUserId>());

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
