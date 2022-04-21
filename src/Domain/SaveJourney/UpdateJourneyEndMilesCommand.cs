// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.SaveJourney;

/// <inheritdoc cref="UpdateJourneyEndMilesHandler"/>
/// <param name="UserId"></param>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="RateId"></param>
public sealed record class UpdateJourneyEndMilesCommand(
	AuthUserId UserId,
	JourneyId Id,
	long Version,
	int EndMiles
) : ICommand, IWithId<JourneyId>;
