// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveJourney;

/// <summary>
/// Save a journey - create if it doesn't exist, or update if it does
/// </summary>
internal sealed class SaveJourneyHandler : QueryHandler<SaveJourneyQuery, JourneyId>
{
	private IDispatcher Dispatcher { get; init; }

	private IJourneyRepository Journey { get; init; }

	private ILog<SaveJourneyHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="dispatcher"></param>
	/// <param name="journey"></param>
	/// <param name="log"></param>
	public SaveJourneyHandler(IDispatcher dispatcher, IJourneyRepository journey, ILog<SaveJourneyHandler> log) =>
		(Dispatcher, Journey, Log) = (dispatcher, journey, log);

	/// <summary>
	/// Save the journey belonging to user specified in <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	/// <exception cref="NotImplementedException"></exception>
	public override Task<Maybe<JourneyId>> HandleAsync(SaveJourneyQuery query)
	{
		throw new NotImplementedException();
	}
}
