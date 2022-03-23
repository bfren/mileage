// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Persistence.Entities.StrongIds;

namespace Queries.DeleteJourney;

/// <summary>
/// Delete a journey
/// </summary>
/// <param name="JourneyId">Journey ID</param>
/// <param name="UserId">User ID</param>
public sealed record class DeleteJourneyQuery(
	JourneyId JourneyId,
	AuthUserId UserId
);
