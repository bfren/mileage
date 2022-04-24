// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveJourney.Internals.UpdateJourneyCommand_Tests;

public class Constructor_Tests
{
	[Theory]
	[InlineData(null, 0L)]
	[InlineData(42L, 42L)]
	public void Receives_SaveJourneyQuery__Sets_Correct_Values(long? queryVersion, long expectedVersion)
	{
		// Arrange
		var journeyId = LongId<JourneyId>();
		var version = queryVersion;
		var day = Rnd.DateTime;
		var carId = LongId<CarId>();
		var startMiles = Rnd.UInt;
		var endMiles = Rnd.UInt;
		var fromPlaceId = LongId<PlaceId>();
		var toPlaceIds = new[] { LongId<PlaceId>(), LongId<PlaceId>() };
		var rateId = LongId<RateId>();
		var saveJourneyQuery = new SaveJourneyQuery(LongId<AuthUserId>(), journeyId, version, day, carId, startMiles, endMiles, fromPlaceId, toPlaceIds, rateId);

		// Act
		var result = new UpdateJourneyCommand(journeyId, saveJourneyQuery);

		// Assert
		Assert.Equal(journeyId, result.JourneyId);
		Assert.Equal(expectedVersion, result.Version);
		Assert.Equal(day, result.Day);
		Assert.Equal(carId, result.CarId);
		Assert.Equal(startMiles, result.StartMiles);
		Assert.Equal(endMiles, result.EndMiles);
		Assert.Equal(fromPlaceId, result.FromPlaceId);
		Assert.Equal(toPlaceIds, result.ToPlaceIds);
		Assert.Equal(rateId, result.RateId);
	}
}
