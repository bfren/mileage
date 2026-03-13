// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data.Common;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Entities;

namespace Mileage.Persistence.Repositories;

/// <summary>
/// Rate repository
/// </summary>
public interface IRateRepository : IRepository<RateEntity, RateId>
{
}
