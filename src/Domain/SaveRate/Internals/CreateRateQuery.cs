// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveRate.Internals;

/// <inheritdoc cref="CreateRateHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="AmountPerMileGBP">Amount per Mile (in GBP)</param>
internal sealed record class CreateRateQuery(
	AuthUserId UserId,
	float AmountPerMileGBP
) : IQuery<RateId>
{
	/// <summary>
	/// Create from <see cref="SaveRateQuery"/>
	/// </summary>
	/// <param name="query"></param>
	public CreateRateQuery(SaveRateQuery query) : this(
		UserId: query.UserId,
		AmountPerMileGBP: query.AmountPerMileGBP
	)
	{ }
}
