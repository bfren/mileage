// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Collections.Generic;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.SaveJourney;

/// <inheritdoc cref="UpdateJourneyToPlacesHandler"/>
/// <param name="UserId"></param>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="ToPlaceIds"></param>
public sealed record class UpdateJourneyToPlacesCommand(
	AuthUserId UserId,
	JourneyId Id,
	long Version,
	List<PlaceId> ToPlaceIds
) : Command, IWithId<JourneyId>, IWithUserId;
