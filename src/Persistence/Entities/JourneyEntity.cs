// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Jeebs.Auth.Data;
using Jeebs.Collections;
using Jeebs.Id;
using Persistence.Entities.StrongIds;

namespace Persistence.Entities;

/// <summary>
/// Journey entity
/// </summary>
public sealed class JourneyEntity : IWithId<JourneyId>
{
	/// <summary>
	/// Journey ID
	/// </summary>
	public JourneyId Id { get; init; }

	/// <summary>
	/// User ID
	/// </summary>
	public AuthUserId UserId { get; init; }

	/// <summary>
	/// Journey Date
	/// </summary>
	public DateOnly Date { get; init; }

	/// <summary>
	/// The car used for the journey
	/// </summary>
	public CarId CarId { get; init; }

	/// <summary>
	/// Start (miles)
	/// </summary>
	public uint StartMiles { get; init; }

	/// <summary>
	/// [Optional] End (miles)
	/// </summary>
	public uint? EndMiles { get; init; }

	/// <summary>
	/// From (start) place
	/// </summary>
	public PlaceId FromPlaceId { get; init; }

	/// <summary>
	/// To (visited) places
	/// </summary>
	public ImmutableList<PlaceId> ToPlaceIds { get; init; } = new();

	/// <summary>
	/// The rate used for the journey
	/// </summary>
	public RateId RateId { get; init; }
}
