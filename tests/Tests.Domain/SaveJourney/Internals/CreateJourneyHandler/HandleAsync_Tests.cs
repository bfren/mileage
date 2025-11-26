// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveJourney.Internals.CreateJourneyHandler_Tests;

public sealed class HandleAsync_Tests : Abstracts.TestHandler
{
	private sealed class Setup : Setup<IJourneyRepository, JourneyEntity, JourneyId, CreateJourneyHandler>
	{
		internal override CreateJourneyHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);
	}

	private (CreateJourneyHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task Calls_Log_Vrb__With_Query()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new CreateJourneyQuery();

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Log.Received().Vrb("Create Journey: {Query}", query);
	}

	[Fact]
	public async Task Calls_Repo_CreateAsync__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var day = Rnd.DateTime;
		var carId = LongId<CarId>();
		var startMiles = Rnd.UInt;
		var endMiles = startMiles + Rnd.UInt;
		var fromPlaceId = LongId<PlaceId>();
		var toPlaceIds = new[] { LongId<PlaceId>(), LongId<PlaceId>() };
		var rateId = LongId<RateId>();
		var query = new CreateJourneyQuery(userId, day, carId, startMiles, endMiles, fromPlaceId, toPlaceIds, rateId);

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Repo.Received().CreateAsync(Arg.Is<JourneyEntity>(j =>
			j.UserId == userId
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
		var expected = LongId<JourneyId>();
		v.Repo.CreateAsync(default!)
			.ReturnsForAnyArgs(expected);
		var query = new CreateJourneyQuery();

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var some = result.AssertSome();
		Assert.Equal(expected, some);
	}
}
