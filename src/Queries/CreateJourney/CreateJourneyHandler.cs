// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Persistence.Entities.StrongIds;
using Persistence.Repositories;

namespace Queries.CreateJourney;

/// <summary>
/// Create a new mileage entity
/// </summary>
public sealed class CreateJourneyHandler : IQueryHandler<CreateJourneyQuery, JourneyId>
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
	/// <param name="cancellationToken"></param>
	public Task<Maybe<JourneyId>> HandleAsync(CreateJourneyQuery query, CancellationToken cancellationToken)
	{
		Log.Dbg("Create Journey: {Query}", query);
		return Journey.CreateAsync(new()
		{
			UserId = query.UserId,
			Date = query.Date,
			CarId = query.CarId,
			StartMiles = query.StartMiles,
			FromPlaceId = query.FromPlaceId
		});
	}
}
