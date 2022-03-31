// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveJourney.Internals;

/// <inheritdoc cref="CreateJourneyHandler"/>
/// <param name="JourneyId">Journey ID</param>
/// <param name="Version">Entity Version</param>
/// <param name="Date">The date of the new journey</param>
/// <param name="CarId">CarId to associate with the new journey</param>
/// <param name="StartMiles">Starting miles</param>
/// <param name="EndMiles">Ending miles</param>
/// <param name="FromPlaceId">Starting place</param>
/// <param name="ToPlaceIds">Visited places</param>
/// <param name="RateId">Rate ID</param>
internal sealed record class UpdateJourneyCommand(
	JourneyId JourneyId,
	long Version,
	DateOnly Date,
	CarId CarId,
	uint StartMiles,
	uint? EndMiles,
	PlaceId FromPlaceId,
	PlaceId[]? ToPlaceIds,
	RateId? RateId
) : ICommand;
