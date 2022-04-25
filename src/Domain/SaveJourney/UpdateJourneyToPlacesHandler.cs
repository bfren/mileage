
// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveJourney;

/// <summary>
/// Update journey To Places
/// </summary>
internal sealed class UpdateJourneyToPlacesHandler : CommandHandler<UpdateJourneyToPlacesCommand>
{
	private IJourneyRepository Journey { get; init; }

	private ILog<UpdateJourneyToPlacesHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="log"></param>
	public UpdateJourneyToPlacesHandler(IJourneyRepository journey, ILog<UpdateJourneyToPlacesHandler> log) =>
		(Journey, Log) = (journey, log);

	/// <summary>
	/// Update journey To Places belonging to user specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(UpdateJourneyToPlacesCommand command)
	{
		Log.Vrb("Updating To Places for {Journey}.", command);
		return Journey.UpdateAsync(command);
	}
}
