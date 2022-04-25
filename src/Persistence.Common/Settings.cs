// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Persistence.Common;

/// <summary>
/// Represents a user's settings
/// </summary>
/// <param name="Id">Settings ID</param>
/// <param name="Version">Version (for concurrency)</param>
/// <param name="DefaultCarId">Default Car ID</param>
/// <param name="DefaultFromPlaceId">Default 'From' Place ID</param>
/// <param name="DefaultRateId">Default Rate ID</param>
public sealed record class Settings(
	SettingsId Id,
	long Version,
	CarId? DefaultCarId,
	PlaceId? DefaultFromPlaceId,
	RateId? DefaultRateId
)
{
	/// <summary>
	/// Create default settings object
	/// </summary>
	public Settings() : this(new(), 0L, null, null, null) { }
}
