// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveJourney;

/// <see cref="SaveJourneyHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="JourneyId">Journey ID</param>
/// <param name="Version">Entity Version</param>
/// <param name="Date">Journey Date</param>
/// <param name="CarId">Car ID</param>
/// <param name="StartMiles">Starting miles</param>
/// <param name="EndMiles">Ending miles</param>
/// <param name="FromPlaceId">Starting place</param>
/// <param name="ToPlaceIds">Places visited</param>
/// <param name="RateId">Rate ID</param>
public sealed record class SaveJourneyQuery(
	AuthUserId UserId,
	JourneyId JourneyId,
	long Version,
	DateOnly Date,
	CarId CarId,
	uint StartMiles,
	uint? EndMiles,
	PlaceId FromPlaceId,
	PlaceId[]? ToPlaceIds,
	RateId? RateId
) : IQuery<JourneyId>;
