// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;

namespace Mileage.Domain.GetUserProfile;

/// <summary>
/// User profile details
/// </summary>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="EmailAddress"></param>
/// <param name="GivenName"></param>
/// <param name="FamilyName"></param>
public sealed record class UserProfileModel(
	AuthUserId Id,
	long Version,
	string EmailAddress,
	string? GivenName,
	string? FamilyName
)
{
	public UserProfileModel() : this(new(), 0L, string.Empty, null, null) { }
}
