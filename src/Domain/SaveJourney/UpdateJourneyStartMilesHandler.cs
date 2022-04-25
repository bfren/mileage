// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveJourney;

/// <summary>
/// Update journey Start Miles
/// </summary>
internal sealed class UpdateJourneyStartMilesHandler : CommandHandler<UpdateJourneyStartMilesCommand>
{
	private IJourneyRepository Journey { get; init; }

	private ILog<UpdateJourneyStartMilesHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="log"></param>
	public UpdateJourneyStartMilesHandler(IJourneyRepository journey, ILog<UpdateJourneyStartMilesHandler> log) =>
		(Journey, Log) = (journey, log);

	/// <summary>
	/// Update journey Start Miles belonging to user specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(UpdateJourneyStartMilesCommand command)
	{
		Log.Vrb("Updating Start Miles for {Journey}.", command);
		return Journey.UpdateAsync(command);
	}
}
