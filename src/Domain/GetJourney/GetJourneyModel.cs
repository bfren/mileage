// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Jeebs.Collections;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.GetJourney;

/// <summary>
/// Journey model
/// </summary>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="Date"></param>
/// <param name="CarId"></param>
/// <param name="StartMiles"></param>
/// <param name="EndMiles"></param>
/// <param name="FromPlaceId"></param>
/// <param name="ToPlaceIds"></param>
/// <param name="RateId"></param>
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
	/// <summary>
	/// Create empty model
	/// </summary>
	public GetJourneyModel() : this(new(), default, default, new(), default, null, new(), new ImmutableList<PlaceId>(), null) { }
}
