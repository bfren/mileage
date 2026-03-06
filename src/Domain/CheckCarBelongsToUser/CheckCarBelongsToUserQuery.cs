// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.Ids;

namespace Mileage.Domain.CheckCarBelongsToUser;

/// <summary>
/// Returns true if <paramref name="CarId"/> belongs to <paramref name="UserId"/>
/// </summary>
/// <param name="UserId">User ID</param>
/// <param name="CarId">Car ID</param>
public sealed record class CheckCarBelongsToUserQuery(
	AuthUserId UserId,
	CarId CarId
) : Query<bool>;
