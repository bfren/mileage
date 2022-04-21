// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveRate;

/// <inheritdoc cref="SaveRateHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="Id">Rate ID</param>
/// <param name="Version">Entity Verion</param>
/// <param name="AmountPerMileGBP">Amount per Mile (in GBP)</param>
public sealed record class SaveRateQuery(
	AuthUserId UserId,
	RateId? Id,
	long Version,
	float AmountPerMileGBP
) : IQuery<RateId>
{
	/// <summary>
	/// Save with minimum required values (for new rates)
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="amountPerMileGBP"></param>
	public SaveRateQuery(AuthUserId userId, float amountPerMileGBP) : this(
		UserId: userId,
		Id: null,
		Version: 0L,
		AmountPerMileGBP: amountPerMileGBP
	)
	{ }

	/// <summary>
	/// Create blank for model binding
	/// </summary>
	public SaveRateQuery() : this(new(), 0f) { }
}
