// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Id;
using Mileage.Persistence.Entities.StrongIds;

namespace Mileage.Persistence.Entities;

/// <summary>
/// Car entity
/// </summary>
public sealed class CarEntity : IWithId<CarId>
{
	/// <summary>
	/// ID
	/// </summary>
	public CarId Id { get; init; }

	/// <summary>
	/// User ID
	/// </summary>
	public AuthUserId UserId { get; init; }

	/// <summary>
	/// Description
	/// </summary>
	public string Description { get; set; } = string.Empty;
}
