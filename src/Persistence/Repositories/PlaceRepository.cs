// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data;
using Jeebs.Logging;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Entities.StrongIds;

namespace Mileage.Persistence.Repositories;

/// <summary>
/// Place repository
/// </summary>
public sealed class PlaceRepository : Repository<PlaceEntity, PlaceId>, IPlaceRepository
{
	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="db">IDb</param>
	/// <param name="log">ILog</param>
	public PlaceRepository(IDb db, ILog<PlaceRepository> log) : base(db, log) { }
}
