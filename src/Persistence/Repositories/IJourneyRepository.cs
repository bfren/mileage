// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;

namespace Mileage.Persistence.Repositories;

/// <summary>
/// Journey repository
/// </summary>
public interface IJourneyRepository : IRepository<JourneyEntity, JourneyId>
{
}
