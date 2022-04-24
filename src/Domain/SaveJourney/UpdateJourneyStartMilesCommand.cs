// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.SaveJourney;

/// <inheritdoc cref="UpdateJourneyStartMilesHandler"/>
/// <param name="UserId"></param>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="StartMiles"></param>
public sealed record class UpdateJourneyStartMilesCommand(
	AuthUserId UserId,
	JourneyId Id,
	long Version,
	int StartMiles
) : WithUserId, ICommand, IWithId<JourneyId>;
