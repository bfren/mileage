// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.SaveRate.Internals;

/// <inheritdoc cref="UpdateRateHandler"/>
/// <param name="Id">Rate ID</param>
/// <param name="Version">Entity Version</param>
/// <param name="AmountPerMileGBP">Amount per Mile (in GBP)</param>
/// <param name="IsDisabled"></param>
internal sealed record class UpdateRateCommand(
	RateId Id,
	long Version,
	float AmountPerMileGBP,
	bool IsDisabled
) : Command, IWithId<RateId>
{
	public UpdateRateCommand(RateId rateId, SaveRateQuery query) : this(
		Id: rateId,
		Version: query.Version,
		AmountPerMileGBP: query.AmountPerMileGBP,
		IsDisabled: query.IsDisabled
	)
	{ }
}
