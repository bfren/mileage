// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Attributes;
using Jeebs.Id;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Persistence.Entities;

/// <summary>
/// Config entity
/// </summary>
public sealed record class SettingsEntity : IWithVersion<SettingsId>
{
	/// <summary>
	/// Config ID
	/// </summary>
	[Id]
	public SettingsId Id { get; init; }

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
	/// Default Car to be used for new journeys
	/// </summary>
	public CarId? DefaultCarId { get; init; }

	/// <summary>
	/// Default Place to be used as the from (start) place for new journeys
	/// </summary>
	public PlaceId? DefaultFromPlaceId { get; set; }
}
