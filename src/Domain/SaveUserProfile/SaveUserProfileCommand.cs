// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Data;

namespace Mileage.Domain.SaveUserProfile;

/// <inheritdoc cref="SaveUserProfileHandler"/>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="EmailAddress"></param>
/// <param name="FriendlyName"></param>
/// <param name="GivenName"></param>
/// <param name="FamilyName"></param>
public sealed record class SaveUserProfileCommand(
	AuthUserId Id,
	long Version,
	string EmailAddress,
	string FriendlyName,
	string? GivenName,
	string? FamilyName
) : Command, IWithVersion<AuthUserId>;
