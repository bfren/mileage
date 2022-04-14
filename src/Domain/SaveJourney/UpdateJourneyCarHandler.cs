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
internal sealed class UpdateJourneyCarHandler : CommandHandler<UpdateJourneyCarCommand>
{
	private IJourneyRepository Journey { get; init; }

	private ILog<UpdateJourneyCarHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="log"></param>
	public UpdateJourneyCarHandler(IJourneyRepository journey, ILog<UpdateJourneyCarHandler> log) =>
		(Journey, Log) = (journey, log);

	/// <summary>
	/// Save the journey belonging to user specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	/// <exception cref="NotImplementedException"></exception>
	public override Task<Maybe<bool>> HandleAsync(UpdateJourneyCarCommand command)
	{
		Log.Vrb("Updating car for {Journey}.", command);
		return Journey.UpdateAsync(command);
	}
}