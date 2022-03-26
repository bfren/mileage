// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.DeleteJourney;

/// <inheritdoc cref="DeleteJourneyHandler"/>
/// <param name="JourneyId">Journey ID</param>
/// <param name="UserId">User ID</param>
public sealed record class DeleteJourneyQuery(
	JourneyId JourneyId,
	AuthUserId UserId
) : IQuery<bool>;
