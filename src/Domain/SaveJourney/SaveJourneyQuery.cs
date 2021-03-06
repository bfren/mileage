// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;
using RndF;

namespace Mileage.Domain.SaveJourney;

/// <inheritdoc cref="SaveJourneyHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="JourneyId">Journey ID</param>
/// <param name="Version">Entity Version</param>
/// <param name="Day">Journey Date</param>
/// <param name="CarId">Car ID</param>
/// <param name="StartMiles">Starting miles</param>
/// <param name="EndMiles">Ending miles</param>
/// <param name="FromPlaceId">Starting place</param>
/// <param name="ToPlaceIds">Places visited</param>
/// <param name="RateId">Rate ID</param>
public sealed record class SaveJourneyQuery(
	AuthUserId UserId,
	JourneyId? JourneyId,
	long? Version,
	DateTime Day,
	CarId CarId,
	uint StartMiles,
	uint? EndMiles,
	PlaceId FromPlaceId,
	PlaceId[]? ToPlaceIds,
	RateId? RateId
) : Query<JourneyId>
{
	/// <summary>
	/// Save with minimum required values (for new journeys)
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="carId"></param>
	/// <param name="startMiles">Starting miles</param>
	/// <param name="fromPlaceId"></param>
	public SaveJourneyQuery(AuthUserId userId, CarId carId, uint startMiles, PlaceId fromPlaceId) :
		this(
			UserId: userId,
			JourneyId: null,
			Version: null,
			Day: DateTime.Today,
			CarId: carId,
			StartMiles: startMiles,
			EndMiles: null,
			FromPlaceId: fromPlaceId,
			ToPlaceIds: null,
			RateId: null
		)
	{ }

	/// <summary>
	/// Create empty for testing and model binding
	/// </summary>
	public SaveJourneyQuery() : this(new(), new(), Rnd.UInt, new()) { }
}
