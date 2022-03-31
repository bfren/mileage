// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveJourney.Internals;

/// <inheritdoc cref="CreateJourneyHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="Date">The date of the new journey</param>
/// <param name="CarId">CarId to associate with the new journey</param>
/// <param name="StartMiles">Starting miles</param>
/// <param name="FromPlaceId">Starting place</param>
internal sealed record class CreateJourneyQuery(
	AuthUserId UserId,
	DateOnly Date,
	CarId CarId,
	uint StartMiles,
	PlaceId FromPlaceId
) : IQuery<JourneyId>;
