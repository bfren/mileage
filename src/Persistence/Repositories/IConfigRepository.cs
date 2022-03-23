// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data;
using Persistence.Entities;
using Persistence.Entities.StrongIds;

namespace Persistence.Repositories;

/// <summary>
/// Config repository
/// </summary>
public interface IConfigRepository : IRepository<ConfigEntity, ConfigId>
{
}
