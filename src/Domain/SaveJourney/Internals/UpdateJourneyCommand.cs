// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;
using RndF;

namespace Mileage.Domain.SaveJourney.Internals;

/// <inheritdoc cref="CreateJourneyHandler"/>
/// <param name="JourneyId">Journey ID</param>
/// <param name="Version">Entity Version</param>
/// <param name="Date">The date of the new journey</param>
/// <param name="CarId">CarId to associate with the new journey</param>
/// <param name="StartMiles">Starting miles</param>
/// <param name="EndMiles">Ending miles</param>
/// <param name="FromPlaceId">Starting place</param>
/// <param name="ToPlaceIds">Visited places</param>
/// <param name="RateId">Rate ID</param>
internal sealed record class UpdateJourneyCommand(
	AuthUserId UserId,
	JourneyId JourneyId,
	long Version,
	DateOnly Date,
	CarId CarId,
	uint StartMiles,
	uint? EndMiles,
	PlaceId FromPlaceId,
	PlaceId[]? ToPlaceIds,
	RateId? RateId
) : ICommand
{
	/// <summary>
	/// Update from a <see cref="SaveJourneyQuery"/>
	/// </summary>
	/// <param name="journeyId">Journey ID - set by <see cref="UpdateJourneyHandler.HandleAsync(UpdateJourneyCommand)"/></param>
	/// <param name="query"></param>
	public UpdateJourneyCommand(JourneyId journeyId, SaveJourneyQuery query) :
		this(
			UserId: query.UserId,
			JourneyId: journeyId,
			Version: query.Version ?? 0L,
			Date: query.Date,
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
	internal UpdateJourneyCommand() : this(new(), new(), Rnd.Lng, Rnd.Date, new(), Rnd.UInt, null, new(), null, null) { }
}
