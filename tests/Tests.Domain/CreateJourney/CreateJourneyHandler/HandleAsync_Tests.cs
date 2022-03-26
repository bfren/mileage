// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.CreateJourney.CreateJourneyHandler_Tests;

public class HandleAsync_Tests : TestHandler<IJourneyRepository, JourneyEntity, JourneyId, CreateJourneyHandler>
{
	public override CreateJourneyHandler GetHandler(Vars v) =>
		new(v.Repo, v.Log);

	[Fact]
	public async Task Calls_Log_Vrb__With_Query()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new CreateJourneyQuery(new(), Rnd.DateF.Get(), new(), Rnd.Uint, new());

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
		var userId = Rnd.Lng;
		var date = Rnd.DateF.Get();
		var carId = Rnd.Lng;
		var startMiles = Rnd.Uint;
		var placeId = Rnd.Uint;
		var query = new CreateJourneyQuery(new(userId), date, new(carId), startMiles, new(placeId));

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Repo.Received().CreateAsync(Arg.Is<JourneyEntity>(j =>
			j.UserId.Value == userId
			&& j.Date == date.ToDateTime(TimeOnly.MinValue)
			&& j.CarId.Value == carId
			&& j.StartMiles == startMiles
			&& j.FromPlaceId.Value == placeId
		));
	}

	[Fact]
	public async void Calls_Repo_CreateAsync__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var expected = new JourneyId(Rnd.Lng);
		v.Repo.CreateAsync(default!)
			.ReturnsForAnyArgs(expected);
		var query = new CreateJourneyQuery(new(), Rnd.DateF.Get(), new(), Rnd.Uint, new());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var some = result.AssertSome();
		Assert.Equal(expected, some);
	}
}
