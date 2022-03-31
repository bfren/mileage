// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveRate.Internals.CreateRate;

/// <inheritdoc cref="CreateRateHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="AmountPerMileGBP">Amount per Mile (in GBP)</param>
public sealed record class CreateRateQuery(
	AuthUserId UserId,
	float AmountPerMileGBP
) : IQuery<RateId>;
