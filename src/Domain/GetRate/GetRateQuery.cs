// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.Ids;

namespace Mileage.Domain.GetRate;

/// <inheritdoc cref="GetRateHandler"/>
/// <param name="UserId"></param>
/// <param name="RateId"></param>
public sealed record class GetRateQuery(
	AuthUserId UserId,
	RateId RateId
) : Query<RateModel>;
