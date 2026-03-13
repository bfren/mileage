// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.Ids;
using Wrap.Ids;

namespace Mileage.Domain.SaveJourney;

/// <inheritdoc cref="UpdateJourneyRateHandler"/>
/// <param name="UserId"></param>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="RateId"></param>
public sealed record class UpdateJourneyRateCommand(
	AuthUserId UserId,
	JourneyId Id,
	long Version,
	RateId RateId
) : Command, IWithId<JourneyId, long>, IWithUserId;
