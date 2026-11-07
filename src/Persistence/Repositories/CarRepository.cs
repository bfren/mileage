// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data.Common;
using Jeebs.Logging;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Entities;

namespace Mileage.Persistence.Repositories;

/// <summary>
/// Car repository
/// </summary>
/// <remarks>
/// Inject dependencies
/// </remarks>
/// <param name="db">IDb</param>
/// <param name="log">ILog</param>
public sealed class CarRepository(IDb db, ILog<CarRepository> log) : Repository<CarEntity, CarId>(db, log), ICarRepository
{
}
