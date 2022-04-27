// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.GetRate;

/// <summary>
/// Rate model
/// </summary>
/// <param name="UserId"></param>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="AmountPerMileGBP"></param>
public sealed record class GetRateModel(
	AuthUserId UserId,
	RateId Id,
	long Version,
	float AmountPerMileGBP
)
{
	/// <summary>
	/// Create blank for model binding
	/// </summary>
	public GetRateModel() : this(new(), new(), 0L, 0f) { }
}
