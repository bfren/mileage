// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Logging;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Queries.CreateJourney.CreateJourneyHandler_Tests;

public class HandleAsync_Tests
{
	private static (IJourneyRepository, ILog<CreateJourneyHandler>, CreateJourneyHandler) Setup()
	{
		var repo = Substitute.For<IJourneyRepository>();
		var log = Substitute.For<ILog<CreateJourneyHandler>>();
		var handler = new CreateJourneyHandler(repo, log);

		return (repo, log, handler);
	}

	public class Calls_Log_Dbg
	{
		[Fact]
		public async Task With_Query()
		{
			// Arrange
			var (_, log, handler) = Setup();
			var query = new CreateJourneyQuery(new(), Rnd.DateF.Get(), new(), Rnd.Uint, new());

			// Act
			await handler.HandleAsync(query, CancellationToken.None);

			// Assert
			log.Received().Dbg("Create Journey: {Query}", query);
		}
	}

	public class Calls_Repo_CreateAsync
	{
		[Fact]
		public async Task With_Correct_Values()
		{
			// Arrange
			var (repo, _, handler) = Setup();
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
				&& j.Date == date.ToDateTime(TimeOnly.MinValue)
				&& j.CarId.Value == carId
				&& j.StartMiles == startMiles
				&& j.FromPlaceId.Value == placeId
			));
		}

		[Fact]
		public async void Returns_Result()
		{
			// Arrange
			var (repo, _, handler) = Setup();
			var expected = new JourneyId(Rnd.Lng);
			repo.CreateAsync(default!)
				.ReturnsForAnyArgs(expected);
			var query = new CreateJourneyQuery(new(), Rnd.DateF.Get(), new(), Rnd.Uint, new());

			// Act
			var result = await handler.HandleAsync(query, CancellationToken.None);

			// Assert
			var some = result.AssertSome();
			Assert.Equal(expected, some);
		}
	}
}
