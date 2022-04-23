// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;

namespace Mileage.Domain;

public abstract record class WithUserId
{
	public abstract AuthUserId UserId { get; init; }
}
