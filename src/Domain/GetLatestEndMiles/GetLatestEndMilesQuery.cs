// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.GetLatestEndMiles;

/// <inheritdoc cref="GetLatestEndMilesHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="CarId">Car ID</param>
public sealed record class GetLatestEndMilesQuery(
	AuthUserId UserId,
	CarId? CarId
) : Query<uint>;
