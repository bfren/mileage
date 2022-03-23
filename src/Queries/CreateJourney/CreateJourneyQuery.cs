// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Jeebs.Auth.Data;
using Persistence.Entities.StrongIds;

namespace Queries.CreateJourney;

/// <summary>
/// Create a new journey
/// </summary>
/// <param name="UserId">User ID</param>
/// <param name="Date">The date of the new journey</param>
/// <param name="CarId">CarId to associate with the new journey</param>
/// <param name="StartMiles">Starting miles</param>
/// <param name="FromPlaceId">Starting place</param>
public sealed record class CreateJourneyQuery(
	AuthUserId UserId,
	DateOnly Date,
	CarId CarId,
	uint StartMiles,
	PlaceId FromPlaceId
);
