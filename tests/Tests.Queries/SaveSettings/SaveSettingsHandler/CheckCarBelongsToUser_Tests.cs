// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using MaybeF;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Queries.SaveSettings.SaveSettingsHandler_Tests;

public class CheckCarBelongsToUser_Tests : TestHandler<ISettingsRepository, SettingsEntity, SettingsId, SaveSettingsHandler>
{
	public override SaveSettingsHandler GetHandler(Vars v) =>
		new(v.Dispatcher, v.Repo, v.Log);

	[Fact]
	public async Task With_CarId__Calls_Dispatcher_DispatchAsync__Receives_Some__Returns_Value()
	{
		// Arrange
		var (handler, v) = GetVars();
		var value = Rnd.Flip;
		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(F.Some(value).AsTask);

		// Act
		var result = await handler.CheckCarBelongsToUser(new(Rnd.Lng), new(Rnd.Lng));

		// Assert
		Assert.Equal(value, result);
	}

	[Fact]
	public async Task With_CarId__Calls_Dispatcher_DispatchAsync__Receives_None__Returns_False()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(Create.None<bool>());

		// Act
		var result = await handler.CheckCarBelongsToUser(new(Rnd.Lng), new(Rnd.Lng));

		// Assert
		Assert.False(result);
	}

	[Fact]
	public async Task Without_CarId__Returns_True()
	{
		// Arrange
		var (handler, v) = GetVars();

		// Act
		var result = await handler.CheckCarBelongsToUser(null, new());

		// Assert
		Assert.True(result);
	}
}
