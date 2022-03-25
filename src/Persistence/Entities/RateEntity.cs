// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Attributes;
using Jeebs.Id;
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
	[Id]
	public RateId Id { get; init; }

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
	/// Mileate rate amount per mile (GBP)
	/// </summary>
	public float AmountPerMileGBP { get; init; }
}
