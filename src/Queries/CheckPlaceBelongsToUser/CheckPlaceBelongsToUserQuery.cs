// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Queries.CheckPlaceBelongsToUser;

/// <summary>
/// Returns true if <paramref name="PlaceId"/> belongs to <paramref name="UserId"/>
/// </summary>
/// <param name="UserId">User ID</param>
/// <param name="PlaceId">Place ID</param>
public sealed record class CheckPlaceBelongsToUserQuery(
	AuthUserId UserId,
	PlaceId PlaceId
) : IQuery<bool>;
