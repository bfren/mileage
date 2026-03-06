// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.Ids;
using Wrap.Ids;

namespace Mileage.Domain.SaveJourney;

/// <inheritdoc cref="UpdateJourneyCarHandler"/>
/// <param name="UserId"></param>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="CarId"></param>
public sealed record class UpdateJourneyCarCommand(
	AuthUserId UserId,
	JourneyId Id,
	long Version,
	CarId CarId
) : Command, IWithId<JourneyId, long>, IWithUserId;
