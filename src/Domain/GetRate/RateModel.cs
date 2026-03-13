// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Mileage.Persistence.Common.Ids;
using Wrap.Ids;

namespace Mileage.Domain.GetRate;

/// <summary>
/// Rate model
/// </summary>
/// <param name="UserId"></param>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="AmountPerMileGBP"></param>
/// <param name="IsDisabled"></param>
public sealed record class RateModel(
	AuthUserId UserId,
	RateId Id,
	long Version,
	float AmountPerMileGBP,
	bool IsDisabled
) : IWithUserId, IWithId<RateId, long>
{
	/// <summary>
	/// Create blank for model binding
	/// </summary>
	public RateModel() : this(new(), new(), 0L, 0f, false) { }
}
