// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.GetJourney;

/// <inheritdoc cref="GetJourneyHandler"/>
/// <param name="UserId"></param>
/// <param name="JourneyId"></param>
public sealed record class GetJourneyQuery(
	AuthUserId UserId,
	JourneyId JourneyId
) : Query<GetJourneyModel>;
