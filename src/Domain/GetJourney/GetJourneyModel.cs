// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Jeebs.Collections;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.GetJourney;

public sealed record class GetJourneyModel(
	JourneyId Id,
	long Version,
	DateTime Date,
	CarId CarId,
	int StartMiles,
	int? EndMiles,
	PlaceId FromPlaceId,
	ImmutableList<PlaceId> ToPlaceIds,
	RateId? RateId
)
{
	public GetJourneyModel() : this(new(), default, default, new(), default, null, new(), new ImmutableList<PlaceId>(), null) { }
}
