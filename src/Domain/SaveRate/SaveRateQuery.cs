// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveRate;

/// <inheritdoc cref="SaveRateHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="RateId">Rate ID</param>
/// <param name="Version">Entity Verion</param>
/// <param name="AmountPerMileInGBP">Amount per Mile (in GBP)</param>
public sealed record class SaveRateQuery(
	AuthUserId UserId,
	RateId? RateId,
	long Version,
	float AmountPerMileInGBP
) : IQuery<RateId>
{
	/// <summary>
	/// Save with minimum required values (for new rates)
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="amountPerMileInGBP"></param>
	public SaveRateQuery(AuthUserId userId, float amountPerMileInGBP) : this(
		UserId: userId,
		RateId: null,
		Version: 0L,
		AmountPerMileInGBP: amountPerMileInGBP
	)
	{ }
}
