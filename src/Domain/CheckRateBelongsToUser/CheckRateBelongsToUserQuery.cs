// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.CheckRateBelongsToUser;

/// <summary>
/// Returns true if <paramref name="RateId"/> belongs to <paramref name="UserId"/>
/// </summary>
/// <param name="UserId">User ID</param>
/// <param name="RateId">Rate ID</param>
public sealed record class CheckRateBelongsToUserQuery(
	AuthUserId UserId,
	RateId RateId
) : Query<bool>;
