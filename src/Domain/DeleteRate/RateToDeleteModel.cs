// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.DeleteRate;

/// <summary>
/// Used to return a place that is ready to be deleted
/// </summary>
/// <param name="Id">Rate ID</param>
/// <param name="Version">Concurrency version</param>
/// <param name="IsDisabled"></param>
internal sealed record class RateToDeleteModel(RateId Id, long Version, bool IsDisabled) : IWithVersion<RateId>
{
	public RateToDeleteModel() : this(new(), 0L, false) { }
}
