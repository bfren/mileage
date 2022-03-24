// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;

namespace Mileage.Persistence.Repositories;

/// <summary>
/// Place repository
/// </summary>
public interface IPlaceRepository : IRepository<PlaceEntity, PlaceId>
{
}
