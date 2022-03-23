// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data;
using Jeebs.Logging;
using Persistence.Entities;
using Persistence.Entities.StrongIds;

namespace Persistence.Repositories;

/// <summary>
/// Car repository
/// </summary>
public sealed class CarRepository : Repository<CarEntity, CarId>, ICarRepository
{
	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="db">IDb</param>
	/// <param name="log">ILog</param>
	public CarRepository(IDb db, ILog<CarRepository> log) : base(db, log) { }
}
