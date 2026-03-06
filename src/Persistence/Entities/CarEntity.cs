// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Data;
using Mileage.Persistence.Common.Ids;

namespace Mileage.Persistence.Entities;

/// <summary>
/// Car entity
/// </summary>
public sealed record class CarEntity : IWithVersion<CarId, long>
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

	/// <summary>
	/// Whether or not this car has been disabled
	/// </summary>
	public bool IsDisabled { get; init; }
}
