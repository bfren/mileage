// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Id;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.DeleteJourney;

/// <summary>
/// Used to return a Journey that is ready to be deleted
/// </summary>
/// <param name="Id">Journey ID</param>
/// <param name="Version">Concurrency version</param>
internal sealed record class JourneyToDelete(JourneyId Id, long Version) : IWithVersion<JourneyId>
{
	public JourneyToDelete() : this(new(), 0L) { }
}
