// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data;
using Mileage.Persistence.Common.Ids;

namespace Mileage.Domain.DeletePlace;

/// <summary>
/// Used to return a place that is ready to be deleted
/// </summary>
/// <param name="Id">Place ID</param>
/// <param name="Version">Concurrency version</param>
/// <param name="IsDisabled"></param>
internal sealed record class PlaceToDeleteModel(PlaceId Id, long Version, bool IsDisabled) : IWithVersion<PlaceId, long>
{
	public PlaceToDeleteModel() : this(new(), 0L, false) { }
}
