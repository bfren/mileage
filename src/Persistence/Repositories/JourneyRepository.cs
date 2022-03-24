// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data;
using Jeebs.Logging;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;

namespace Mileage.Persistence.Repositories;

/// <summary>
/// Config repository
/// </summary>
public sealed class JourneyRepository : Repository<JourneyEntity, JourneyId>, IJourneyRepository
{
	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="db">IDb</param>
	/// <param name="log">ILog</param>
	public JourneyRepository(IDb db, ILog<JourneyRepository> log) : base(db, log) { }
}
