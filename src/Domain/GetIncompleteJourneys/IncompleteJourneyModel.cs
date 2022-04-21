// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Jeebs.Collections;
using Jeebs.Data;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;

namespace Mileage.Domain.GetIncompleteJourneys;

/// <inheritdoc cref="GetIncompleteJourneysQuery"/>
public sealed record class IncompleteJourneyModel : IWithVersion<JourneyId>
{
	/// <inheritdoc cref="JourneyEntity.Id"/>
	public JourneyId Id { get; init; } = new();

	/// <inheritdoc cref="JourneyEntity.Version"/>
	public long Version { get; init; }

	/// <inheritdoc cref="JourneyEntity.Day"/>
	public DateTime Day { get; init; }

	/// <inheritdoc cref="JourneyEntity.CarId"/>
	public CarId CarId { get; init; } = new();

	/// <inheritdoc cref="JourneyEntity.FromPlaceId"/>
	public PlaceId FromPlaceId { get; init; } = new();

	/// <inheritdoc cref="JourneyEntity.ToPlaceIds"/>
	public ImmutableList<PlaceId> ToPlaceIds { get; init; } = new();

	/// <inheritdoc cref="JourneyEntity.StartMiles"/>
	public int StartMiles { get; init; }

	/// <inheritdoc cref="JourneyEntity.EndMiles"/>
	public int? EndMiles { get; set; }

	/// <inheritdoc cref="JourneyEntity.RateId"/>
	public RateId RateId { get; init; } = new();
}
