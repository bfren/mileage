// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveJourney.Internals;

/// <summary>
/// Create a new mileage entity
/// </summary>
internal sealed class CreateJourneyHandler : QueryHandler<CreateJourneyQuery, JourneyId>
{
	private IJourneyRepository Journey { get; init; }

	private ILog<CreateJourneyHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="log"></param>
	public CreateJourneyHandler(IJourneyRepository journey, ILog<CreateJourneyHandler> log) =>
		(Journey, Log) = (journey, log);

	/// <summary>
	/// Create a new journey from <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<JourneyId>> HandleAsync(CreateJourneyQuery query)
	{
		Log.Vrb("Create Journey: {Query}", query);
		return Journey
			.CreateAsync(new()
			{
				UserId = query.UserId,
				Day = query.Day,
				CarId = query.CarId,
				StartMiles = (int)query.StartMiles,
				EndMiles = (int?)query.EndMiles,
				FromPlaceId = query.FromPlaceId,
				ToPlaceIds = query.ToPlaceIds ?? Array.Empty<PlaceId>(),
				RateId = query.RateId
			});
	}
}
