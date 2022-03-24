// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Queries.DeleteJourney;

/// <summary>
/// Used to return a Journey that is ready to be deleted
/// </summary>
/// <param name="Id">Journey ID</param>
internal sealed record class JourneyToDelete(JourneyId Id);
