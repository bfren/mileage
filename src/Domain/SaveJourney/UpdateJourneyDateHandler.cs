// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveJourney;

/// <summary>
/// Save a journey - create if it doesn't exist, or update if it does
/// </summary>
internal sealed class UpdateJourneyDateHandler : CommandHandler<UpdateJourneyDateCommand>
{
	private IJourneyRepository Journey { get; init; }

	private ILog<UpdateJourneyDateHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="log"></param>
	public UpdateJourneyDateHandler(IJourneyRepository journey, ILog<UpdateJourneyDateHandler> log) =>
		(Journey, Log) = (journey, log);

	/// <summary>
	/// Save the journey belonging to user specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	/// <exception cref="NotImplementedException"></exception>
	public override Task<Maybe<bool>> HandleAsync(UpdateJourneyDateCommand command)
	{
		Log.Vrb("Updating Date for {Journey}.", command);
		return Journey.UpdateAsync(command);
	}
}
