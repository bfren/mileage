// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Logging;
using Persistence.Entities;
using Persistence.Repositories;

namespace Queries.CreateJourney.CreateJourneyHandler_Tests;

public class HandleAsync_Tests
{
	[Fact]
	public async Task Logs_To_Dbg()
	{
		// Arrange
		var repo = Substitute.For<IJourneyRepository>();
		var log = Substitute.For<ILog<CreateJourneyHandler>>();
		var handler = new CreateJourneyHandler(repo, log);
		var query = new CreateJourneyQuery(new(), Rnd.DateF.Get(), new(), Rnd.Uint, new());

		// Act
		await handler.HandleAsync(query, CancellationToken.None);

		// Assert
		log.Received().Dbg("Create Journey: {Query}", query);
	}

	[Fact]
	public async Task Calls_Repo_CreateAsync_With_Correct_Values()
	{
		// Arrange
		var repo = Substitute.For<IJourneyRepository>();
		var log = Substitute.For<ILog<CreateJourneyHandler>>();
		var handler = new CreateJourneyHandler(repo, log);
		var userId = Rnd.Lng;
		var date = Rnd.DateF.Get();
		var carId = Rnd.Lng;
		var startMiles = Rnd.Uint;
		var placeId = Rnd.Uint;
		var query = new CreateJourneyQuery(new(userId), date, new(carId), startMiles, new(placeId));

		// Act
		await handler.HandleAsync(query, CancellationToken.None);

		// Assert
		await repo.Received().CreateAsync(Arg.Is<JourneyEntity>(j =>
			j.UserId.Value == userId
			&& j.Date == date
			&& j.CarId.Value == carId
			&& j.StartMiles == startMiles
			&& j.FromPlaceId.Value == placeId
		));
	}
}
