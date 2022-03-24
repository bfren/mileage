// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Entities.StrongIds;

namespace Mileage.Persistence.Repositories;

/// <summary>
/// Rate repository
/// </summary>
public interface IRateRepository : IRepository<RateEntity, RateId>
{
}
