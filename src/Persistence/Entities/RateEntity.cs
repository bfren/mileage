// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Id;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Persistence.Entities;

/// <summary>
/// Rate entity
/// </summary>
public sealed class RateEntity : IWithId<RateId>
{
	/// <summary>
	/// Rate ID
	/// </summary>
	public RateId Id { get; init; }

	/// <summary>
	/// User ID
	/// </summary>
	public AuthUserId UserId { get; init; }

	/// <summary>
	/// Mileate rate amount per mile (GBP)
	/// </summary>
	public float AmountPerMileGBP { get; init; }
}
