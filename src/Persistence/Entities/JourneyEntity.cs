// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Jeebs.Auth.Data;
using Jeebs.Collections;
using Jeebs.Data;
using Jeebs.Data.Attributes;
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
	[Id]
	public JourneyId Id { get; init; } = new();

	/// <summary>
	/// Version
	/// </summary>
	[Version]
	public long Version { get; set; }

	/// <summary>
	/// User ID
	/// </summary>
	public AuthUserId UserId { get; init; } = new();

	/// <summary>
	/// Journey Date
	/// </summary>
	public DateTime Date { get; init; }

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
	public IImmutableList<PlaceId> ToPlaceIds { get; init; } = new ImmutableList<PlaceId>();

	/// <summary>
	/// The rate used for the journey
	/// </summary>
	public RateId? RateId { get; init; } = new();
}
