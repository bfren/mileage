// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data;
using Jeebs.Logging;
using Persistence.Entities;
using Persistence.Entities.StrongIds;

namespace Persistence.Repositories;

/// <summary>
/// Rate repository
/// </summary>
public sealed class RateRepository : Repository<RateEntity, RateId>, IRateRepository
{
	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="db">IDb</param>
	/// <param name="log">ILog</param>
	public RateRepository(IDb db, ILog<RateRepository> log) : base(db, log) { }
}
