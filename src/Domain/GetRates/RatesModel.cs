// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.GetRates;

/// <summary>
/// Rate model
/// </summary>
/// <param name="Id"></param>
/// <param name="AmountPerMileGBP"></param>
/// <param name="IsDisabled"></param>
public sealed record class RatesModel(
	RateId Id,
	float AmountPerMileGBP,
	bool IsDisabled
) : IWithId<RateId>
{
	/// <summary>
	/// Create blank for model binding
	/// </summary>
	public RatesModel() : this(new(), 0f, false) { }
}
