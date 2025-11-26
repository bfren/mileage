// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveJourney.Internals.UpdateJourneyHandler_Tests;

public sealed class HandleAsync_Tests : Abstracts.TestHandler
{
	private sealed class Setup : Setup<IJourneyRepository, JourneyEntity, JourneyId, UpdateJourneyHandler>
	{
		internal override UpdateJourneyHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);
	}

	private (UpdateJourneyHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task Calls_Log_Vrb__With_Query()
	{
		// Arrange
		var (handler, v) = GetVars();
		var command = new UpdateJourneyCommand();

		// Act
		await handler.HandleAsync(command);

		// Assert
		v.Log.Received().Vrb("Update Journey: {Command}", command);
	}

	[Fact]
	public async Task Calls_Repo_CreateAsync__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var journeyId = LongId<JourneyId>();
		var version = Rnd.Lng;
		var day = Rnd.DateTime;
		var carId = LongId<CarId>();
		var startMiles = Rnd.UInt;
		var endMiles = startMiles + Rnd.UInt;
		var fromPlaceId = LongId<PlaceId>();
		var toPlaceIds = new[] { LongId<PlaceId>(), LongId<PlaceId>() };
		var rateId = LongId<RateId>();
		var command = new UpdateJourneyCommand(journeyId, version, day, carId, startMiles, endMiles, fromPlaceId, toPlaceIds, rateId);

		// Act
		await handler.HandleAsync(command);

		// Assert
		await v.Repo.Received().UpdateAsync(Arg.Is<JourneyEntity>(j =>
			j.Id == journeyId
			&& j.Version == version
			&& j.Day == day
			&& j.CarId == carId
			&& j.StartMiles == startMiles
			&& j.EndMiles == endMiles
			&& j.FromPlaceId == fromPlaceId
			&& j.ToPlaceIds.SequenceEqual(toPlaceIds)
			&& j.RateId == rateId
		));
	}

	[Fact]
	public async Task Calls_Repo_CreateAsync__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var expected = Rnd.Flip;
		v.Repo.UpdateAsync<JourneyEntity>(default!)
			.ReturnsForAnyArgs(expected);
		var command = new UpdateJourneyCommand();

		// Act
		var result = await handler.HandleAsync(command);

		// Assert
		var some = result.AssertSome();
		Assert.Equal(expected, some);
	}
}
