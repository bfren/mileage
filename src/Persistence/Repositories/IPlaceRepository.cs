// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data.Common;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Entities;

namespace Mileage.Persistence.Repositories;

/// <summary>
/// Place repository
/// </summary>
public interface IPlaceRepository : IRepository<PlaceEntity, PlaceId>
{
}
