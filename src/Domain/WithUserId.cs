// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;

namespace Mileage.Domain;

/// <summary>
/// Specifies a class has a 'UserId' property
/// </summary>
public abstract record class WithUserId
{
	/// <summary>
	/// User ID
	/// </summary>
	public abstract AuthUserId UserId { get; init; }
}
