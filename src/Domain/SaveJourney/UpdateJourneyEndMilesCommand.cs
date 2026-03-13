// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.Ids;
using Wrap.Ids;

namespace Mileage.Domain.SaveJourney;

/// <inheritdoc cref="UpdateJourneyEndMilesHandler"/>
/// <param name="UserId"></param>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="EndMiles"></param>
public sealed record class UpdateJourneyEndMilesCommand(
	AuthUserId UserId,
	JourneyId Id,
	long Version,
	int EndMiles
) : Command, IWithId<JourneyId, long>, IWithUserId;
