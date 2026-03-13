// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.Ids;

namespace Mileage.Domain.CheckPlacesBelongToUser;

/// <summary>
/// Returns true if <paramref name="PlaceIds"/> belong to <paramref name="UserId"/>
/// </summary>
/// <param name="UserId">User ID</param>
/// <param name="PlaceIds">Place IDs</param>
public sealed record class CheckPlacesBelongToUserQuery(
	AuthUserId UserId,
	params PlaceId[] PlaceIds
) : Query<bool>;
