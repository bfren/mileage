// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.DeleteJourney;

/// <inheritdoc cref="DeleteJourneyHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="JourneyId">Journey ID</param>
public sealed record class DeleteJourneyCommand(
	AuthUserId UserId,
	JourneyId JourneyId
) : ICommand;
