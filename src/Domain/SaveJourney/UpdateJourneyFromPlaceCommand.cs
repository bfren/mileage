// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.SaveJourney;

/// <inheritdoc cref="UpdateJourneyFromPlaceHandler"/>
/// <param name="UserId"></param>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="FromPlaceId"></param>
public sealed record class UpdateJourneyFromPlaceCommand(
	AuthUserId UserId,
	JourneyId Id,
	long Version,
	PlaceId FromPlaceId
) : Command, IWithId<JourneyId>, IWithUserId;
