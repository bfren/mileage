// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Persistence.Entities;

/// <summary>
/// Rate entity
/// </summary>
public sealed record class RateEntity : IWithVersion<RateId>
{
	/// <summary>
	/// Rate ID
	/// </summary>
	public RateId Id { get; init; } = new();

	/// <summary>
	/// Version
	/// </summary>
	public long Version { get; set; }

	/// <summary>
	/// User ID
	/// </summary>
	public AuthUserId UserId { get; init; } = new();

	/// <summary>
	/// Mileate rate amount per mile (GBP)
	/// </summary>
	public float AmountPerMileGBP { get; init; }

	/// <summary>
	/// Whether or not this rate has been disabled
	/// </summary>
	public bool IsDisabled { get; init; }
}
