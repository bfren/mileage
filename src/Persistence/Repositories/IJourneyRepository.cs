// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data.Common;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Entities;

namespace Mileage.Persistence.Repositories;

/// <summary>
/// Journey repository
/// </summary>
public interface IJourneyRepository : IRepository<JourneyEntity, JourneyId>
{
}
