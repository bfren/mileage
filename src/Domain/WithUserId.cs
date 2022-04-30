// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;

namespace Mileage.Domain;

/// <summary>
/// Specifies a class has a 'UserId' property
/// </summary>
public interface IWithUserId
{
	/// <summary>
	/// User ID
	/// </summary>
	AuthUserId UserId { get; init; }
}

/// <inheritdoc cref="IWithUserId"/>
public abstract record class WithUserId : IWithUserId
{
	/// <inheritdoc/>
	public abstract AuthUserId UserId { get; init; }
}
