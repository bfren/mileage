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
/// <param name="FriendlyName"></param>
/// <param name="GivenName"></param>
/// <param name="FamilyName"></param>
public sealed record class UserProfileModel(
	AuthUserId Id,
	long Version,
	string EmailAddress,
	string FriendlyName,
	string? GivenName,
	string? FamilyName
)
{
	/// <summary>
	/// Create empty for model binding
	/// </summary>
	public UserProfileModel() : this(new(), 0L, string.Empty, string.Empty, null, null) { }
}
