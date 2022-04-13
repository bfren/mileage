// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.SaveJourney;

public sealed record class UpdateJourneyFromPlaceCommand(
	AuthUserId UserId,
	JourneyId Id,
	long Version,
	PlaceId FromPlaceId
) : ICommand, IWithId<JourneyId>;
