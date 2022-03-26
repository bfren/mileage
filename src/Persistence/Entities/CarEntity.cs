// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Attributes;
using Jeebs.Id;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Persistence.Entities;

/// <summary>
/// Car entity
/// </summary>
public sealed record class CarEntity : IWithVersion<CarId>
{
	/// <summary>
	/// ID
	/// </summary>
	[Id]
	public CarId Id { get; init; }

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
	/// Description
	/// </summary>
	public string Description { get; init; } = string.Empty;
}
