// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveJourney.SaveJourneyQuery_Tests;

public class Constructor_Tests
{
	[Fact]
	public void Sets_Correct_Values()
	{
		// Arrange
		var userId = LongId<AuthUserId>();
		var carId = LongId<CarId>();
		var startMiles = Rnd.UInt;
		var placeId = LongId<PlaceId>();

		// Act
		var result = new SaveJourneyQuery(userId, carId, startMiles, placeId);

		// Assert
		Assert.Equal(userId, result.UserId);
		Assert.Null(result.JourneyId);
		Assert.Null(result.Version);
		Assert.Equal(DateTime.Today, result.Day);
		Assert.Equal(carId, result.CarId);
		Assert.Equal(startMiles, result.StartMiles);
		Assert.Null(result.EndMiles);
		Assert.Equal(placeId, result.FromPlaceId);
		Assert.Null(result.ToPlaceIds);
		Assert.Null(result.RateId);
	}
}
