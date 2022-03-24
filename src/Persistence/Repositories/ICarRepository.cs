// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Entities.StrongIds;

namespace Mileage.Persistence.Repositories;

/// <summary>
/// Car repository
/// </summary>
public interface ICarRepository : IRepository<CarEntity, CarId>
{
}
