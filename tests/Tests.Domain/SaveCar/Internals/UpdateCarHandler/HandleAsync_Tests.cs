// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveCar.Internals.UpdateCarHandler_Tests;

public class HandleAsync_Tests : Abstracts.TestHandler
{
	private class Setup : Setup<ICarRepository, CarEntity, CarId, UpdateCarHandler>
	{
		internal override UpdateCarHandler GetHandler(Vars v) =>
			new(v.Cache, v.Repo, v.Log);
	}

	private (UpdateCarHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task Calls_Log_Vrb__With_Query()
	{
		// Arrange
		var (handler, v) = GetVars();
		var command = new UpdateCarCommand(LongId<CarId>(), Rnd.Lng, Rnd.Str, Rnd.Str, Rnd.Flip);

		// Act
		await handler.HandleAsync(command);

		// Assert
		v.Log.Received().Vrb("Update Car: {Command}", command);
	}

	[Fact]
	public async Task Calls_Repo_CreateAsync__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var carId = LongId<CarId>();
		var version = Rnd.Lng;
		var description = Rnd.Str;
		var plate = Rnd.Str;
		var disabled = Rnd.Flip;
		var command = new UpdateCarCommand(carId, version, description, plate, disabled);

		// Act
		await handler.HandleAsync(command);

		// Assert
		await v.Repo.Received().UpdateAsync(command);
	}

	[Fact]
	public async void If_Successful__Calls_Cache_RemoveEntry__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var command = new UpdateCarCommand(LongId<CarId>(), Rnd.Lng, Rnd.Str, Rnd.Str, Rnd.Flip);
		v.Repo.UpdateAsync(command)
			.Returns(F.True);

		// Act
		await handler.HandleAsync(command);

		// Assert
		v.Cache.Received().RemoveValue(command.Id);
	}

	[Fact]
	public async void Calls_Repo_CreateAsync__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var expected = Rnd.Flip;
		v.Repo.UpdateAsync<UpdateCarCommand>(default!)
			.ReturnsForAnyArgs(expected);
		var command = new UpdateCarCommand(LongId<CarId>(), Rnd.Lng, Rnd.Str, Rnd.Str, Rnd.Flip);

		// Act
		var result = await handler.HandleAsync(command);

		// Assert
		var some = result.AssertSome();
		Assert.Equal(expected, some);
	}
}
