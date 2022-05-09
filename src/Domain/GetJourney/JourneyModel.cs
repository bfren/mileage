// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Jeebs.Auth.Data;
using Jeebs.Collections;
using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.GetJourney;

/// <summary>
/// Journey model
/// </summary>
/// <param name="UserId"></param>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="Day"></param>
/// <param name="CarId"></param>
/// <param name="StartMiles"></param>
/// <param name="EndMiles"></param>
/// <param name="FromPlaceId"></param>
/// <param name="ToPlaceIds"></param>
/// <param name="RateId"></param>
public sealed record class JourneyModel(
	AuthUserId UserId,
	JourneyId Id,
	long Version,
	DateTime Day,
	CarId CarId,
	int StartMiles,
	int? EndMiles,
	PlaceId FromPlaceId,
	ImmutableList<PlaceId> ToPlaceIds,
	RateId? RateId
) : IWithUserId, IWithId<JourneyId>
{
	/// <summary>
	/// Create empty model
	/// </summary>
	public JourneyModel() : this(new(), new(), default, default, new(), default, null, new(), new ImmutableList<PlaceId>(), null) { }
}
