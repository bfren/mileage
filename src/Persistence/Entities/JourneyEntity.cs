// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Jeebs.Auth.Data;
using Jeebs.Collections;
using Jeebs.Data.Attributes;
using Jeebs.Id;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Persistence.Entities;

/// <summary>
/// Journey entity
/// </summary>
public sealed class JourneyEntity : IWithVersion<JourneyId>
{
	/// <summary>
	/// Journey ID
	/// </summary>
	[Id]
	public JourneyId Id { get; init; }

	/// <summary>
	/// Version
	/// </summary>
	[Version]
	public long Version { get; set; }

	/// <summary>
	/// User ID
	/// </summary>
	public AuthUserId UserId { get; init; }

	/// <summary>
	/// Journey Date
	/// </summary>
	public DateTime Date { get; init; }

	/// <summary>
	/// The car used for the journey
	/// </summary>
	public CarId CarId { get; init; }

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
