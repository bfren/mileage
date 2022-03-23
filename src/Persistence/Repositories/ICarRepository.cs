// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data;
using Persistence.Entities;
using Persistence.Entities.StrongIds;

namespace Persistence.Repositories;

/// <summary>
/// Car repository
/// </summary>
public interface ICarRepository : IRepository<CarEntity, CarId>
{
}
