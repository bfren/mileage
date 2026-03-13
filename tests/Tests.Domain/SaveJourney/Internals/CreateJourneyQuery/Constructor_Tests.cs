// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Mileage.Persistence.Common.Ids;

namespace Mileage.Domain.SaveJourney.Internals.CreateJourneyQuery_Tests;

public class Constructor_Tests
{
	[Fact]
	public void Receives_SaveJourneyQuery__Sets_Correct_Values()
	{
		// Arrange
		var userId = IdGen.LongId<AuthUserId>();
		var day = Rnd.DateTime;
		var carId = IdGen.LongId<CarId>();
		var startMiles = Rnd.UInt32;
		var endMiles = Rnd.UInt32;
		var fromPlaceId = IdGen.LongId<PlaceId>();
		var toPlaceIds = new[] { IdGen.LongId<PlaceId>(), IdGen.LongId<PlaceId>() };
		var rateId = IdGen.LongId<RateId>();
		var saveJourneyQuery = new SaveJourneyQuery(userId, null, null, day, carId, startMiles, endMiles, fromPlaceId, toPlaceIds, rateId);

		// Act
		var result = new CreateJourneyQuery(saveJourneyQuery);

		// Assert
		Assert.Equal(userId, result.UserId);
		Assert.Equal(day, result.Day);
		Assert.Equal(carId, result.CarId);
		Assert.Equal(startMiles, result.StartMiles);
		Assert.Equal(endMiles, result.EndMiles);
		Assert.Equal(fromPlaceId, result.FromPlaceId);
		Assert.Equal(toPlaceIds, result.ToPlaceIds);
		Assert.Equal(rateId, result.RateId);
	}
}
