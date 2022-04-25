// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveJourney;

/// <summary>
/// Update journey End Miles
/// </summary>
internal sealed class UpdateJourneyEndMilesHandler : CommandHandler<UpdateJourneyEndMilesCommand>
{
	private IJourneyRepository Journey { get; init; }

	private ILog<UpdateJourneyEndMilesHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="log"></param>
	public UpdateJourneyEndMilesHandler(IJourneyRepository journey, ILog<UpdateJourneyEndMilesHandler> log) =>
		(Journey, Log) = (journey, log);

	/// <summary>
	/// Update journey End Miles belonging to user specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(UpdateJourneyEndMilesCommand command)
	{
		Log.Vrb("Updating End Miles for {Journey}.", command);
		return Journey.UpdateAsync(command);
	}
}
