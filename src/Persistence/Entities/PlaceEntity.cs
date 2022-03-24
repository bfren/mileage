// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Id;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Persistence.Entities;

/// <summary>
/// Place entity
/// </summary>
public sealed class PlaceEntity : IWithId<PlaceId>
{
	/// <summary>
	/// Place ID
	/// </summary>
	public PlaceId Id { get; init; }

	/// <summary>
	/// User ID
	/// </summary>
	public AuthUserId UserId { get; init; }

	/// <summary>
	/// Description
	/// </summary>
	public string Description { get; init; } = string.Empty;

	/// <summary>
	/// [Optional] Postcode
	/// </summary>
	public string? Postcode { get; init; }
}
