// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.SaveJourney;

public sealed record class UpdateJourneyCarCommand(
	AuthUserId UserId,
	JourneyId Id,
	long Version,
	CarId CarId
) : ICommand, IWithId<JourneyId>;
