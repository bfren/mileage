// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Jeebs.Auth.Data;
using Jeebs.Collections;
using Jeebs.Data;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Persistence.Entities;

/// <summary>
/// Journey entity
/// </summary>
public sealed record class JourneyEntity : IWithVersion<JourneyId>
{
	/// <summary>
	/// Journey ID
	/// </summary>
	public JourneyId Id { get; init; } = new();

	/// <summary>
	/// Version
	/// </summary>
	public long Version { get; set; }

	/// <summary>
	/// User ID
	/// </summary>
	public AuthUserId UserId { get; init; } = new();

	/// <summary>
	/// Journey Day
	/// </summary>
	public DateTime Day { get; init; }

	/// <summary>
	/// The car used for the journey
	/// </summary>
	public CarId CarId { get; init; } = new();

	/// <summary>
	/// Start (miles)
	/// </summary>
	public int StartMiles { get; init; }

	/// <summary>
	/// [Optional] End (miles)
	/// </summary>
	public int? EndMiles { get; init; }

	/// <summary>
	/// From (start) place
	/// </summary>
	public PlaceId FromPlaceId { get; init; } = new();

	/// <summary>
	/// To (visited) places
	/// </summary>
	public ImmutableList<PlaceId> ToPlaceIds { get; init; } = new();

	/// <summary>
	/// The rate used for the journey
	/// </summary>
	public RateId? RateId { get; init; } = new();
}
