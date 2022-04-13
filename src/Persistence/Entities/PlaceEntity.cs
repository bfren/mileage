// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Persistence.Entities;

/// <summary>
/// Place entity
/// </summary>
public sealed record class PlaceEntity : IWithVersion<PlaceId>
{
	/// <summary>
	/// Place ID
	/// </summary>
	public PlaceId Id { get; init; } = new();

	/// <summary>
	/// Version
	/// </summary>
	public long Version { get; set; }

	/// <summary>
	/// User ID
	/// </summary>
	public AuthUserId UserId { get; init; } = new();

	/// <summary>
	/// Description
	/// </summary>
	public string Description { get; init; } = string.Empty;

	/// <summary>
	/// [Optional] Postcode
	/// </summary>
	public string? Postcode { get; init; }
}
