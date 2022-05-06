// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;
using RndF;

namespace Mileage.Domain.SaveJourney.Internals;

/// <inheritdoc cref="CreateJourneyHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="Day">Journey Date</param>
/// <param name="CarId">Car ID</param>
/// <param name="StartMiles">Starting miles</param>
/// <param name="EndMiles">Ending miles</param>
/// <param name="FromPlaceId">Starting place</param>
/// <param name="ToPlaceIds">Places visited</param>
/// <param name="RateId">Rate ID</param>
internal sealed record class CreateJourneyQuery(
	AuthUserId UserId,
	DateTime Day,
	CarId CarId,
	uint StartMiles,
	uint? EndMiles,
	PlaceId FromPlaceId,
	PlaceId[]? ToPlaceIds,
	RateId? RateId
) : Query<JourneyId>
{
	/// <summary>
	/// Create from a <see cref="SaveJourneyQuery"/>
	/// </summary>
	/// <param name="query"></param>
	public CreateJourneyQuery(SaveJourneyQuery query) :
		this(
			UserId: query.UserId,
			Day: query.Day,
			CarId: query.CarId,
			StartMiles: query.StartMiles,
			EndMiles: query.EndMiles,
			FromPlaceId: query.FromPlaceId,
			ToPlaceIds: query.ToPlaceIds,
			RateId: query.RateId
		)
	{ }

	/// <summary>
	/// Allows quick creation in testing
	/// </summary>
	internal CreateJourneyQuery() : this(new(), Rnd.DateTime, new(), Rnd.UInt, null, new(), null, null) { }
}
