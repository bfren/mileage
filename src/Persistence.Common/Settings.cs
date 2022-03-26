// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Persistence.Common;

/// <summary>
/// Represents a user's settings
/// </summary>
/// <param name="Version">Version (for concurrency)</param>
/// <param name="DefaultCarId">Default Car ID</param>
/// <param name="DefaultFromPlaceId">Default 'From' Place ID</param>
public sealed record class Settings(
	long Version,
	CarId? DefaultCarId,
	PlaceId? DefaultFromPlaceId
)
{
	/// <summary>
	/// Create default settings object
	/// </summary>
	public Settings() : this(0, null, null) { }
}
