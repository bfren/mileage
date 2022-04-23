// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;

namespace Mileage.Domain.GetUserProfile;

/// <inheritdoc cref="GetUserProfileHandler"/>
/// <param name="Id"></param>
public sealed record class GetUserProfileQuery(
	AuthUserId Id
) : IQuery<UserProfileModel>;
