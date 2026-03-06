// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Mileage.Persistence.Common.Ids;

namespace Mileage.Domain.SaveJourney.Internals.UpdateJourneyCommand_Tests;

public class Constructor_Tests
{
	[Theory]
	[InlineData(null, 0L)]
	[InlineData(42L, 42L)]
	public void Receives_SaveJourneyQuery__Sets_Correct_Values(long? queryVersion, long expectedVersion)
	{
		// Arrange
		var journeyId = IdGen.LongId<JourneyId>();
		var version = queryVersion;
		var day = Rnd.DateTime;
		var carId = IdGen.LongId<CarId>();
		var startMiles = Rnd.UInt32;
		var endMiles = Rnd.UInt32;
		var fromPlaceId = IdGen.LongId<PlaceId>();
		var toPlaceIds = new[] { IdGen.LongId<PlaceId>(), IdGen.LongId<PlaceId>() };
		var rateId = IdGen.LongId<RateId>();
		var saveJourneyQuery = new SaveJourneyQuery(IdGen.LongId<AuthUserId>(), journeyId, version, day, carId, startMiles, endMiles, fromPlaceId, toPlaceIds, rateId);

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
