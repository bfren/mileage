// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;

namespace Mileage.Persistence.Repositories;

/// <summary>
/// Rate repository
/// </summary>
public interface IRateRepository : IRepository<RateEntity, RateId>
{
}
