// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data;
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
	public CarId Id { get; init; } = new();

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
	/// Number Plate
	/// </summary>
	public string? NumberPlate { get; init; }
}
