// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data.Common;
using Jeebs.Logging;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Entities;

namespace Mileage.Persistence.Repositories;

/// <summary>
/// Settings repository
/// </summary>
/// <remarks>
/// Inject dependencies
/// </remarks>
/// <param name="db">IDb</param>
/// <param name="log">ILog</param>
public sealed class SettingsRepository(IDb db, ILog<SettingsRepository> log) : Repository<SettingsEntity, SettingsId>(db, log), ISettingsRepository
{
}
