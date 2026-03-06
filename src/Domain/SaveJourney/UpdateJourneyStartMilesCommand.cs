// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.Ids;
using Wrap.Ids;

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
) : Command, IWithId<JourneyId, long>, IWithUserId;
