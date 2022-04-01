// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveJourney.Internals.CreateJourneyQuery_Tests;

public class Constructor_Tests
{
	[Fact]
	public void Receives_SaveJourneyQuery__Sets_Correct_Values()
	{
		// Arrange
		var userId = LongId<AuthUserId>();
		var date = Rnd.Date;
		var carId = LongId<CarId>();
		var startMiles = Rnd.UInt;
		var endMiles = Rnd.UInt;
		var fromPlaceId = LongId<PlaceId>();
		var toPlaceIds = new[] { LongId<PlaceId>(), LongId<PlaceId>() };
		var rateId = LongId<RateId>();
		var saveJourneyQuery = new SaveJourneyQuery(userId, null, null, date, carId, startMiles, endMiles, fromPlaceId, toPlaceIds, rateId);

		// Act
		var result = new CreateJourneyQuery(saveJourneyQuery);

		// Assert
		Assert.Equal(userId, result.UserId);
		Assert.Equal(date, result.Date);
		Assert.Equal(carId, result.CarId);
		Assert.Equal(startMiles, result.StartMiles);
		Assert.Equal(endMiles, result.EndMiles);
		Assert.Equal(fromPlaceId, result.FromPlaceId);
		Assert.Equal(toPlaceIds, result.ToPlaceIds);
		Assert.Equal(rateId, result.RateId);
	}
}
