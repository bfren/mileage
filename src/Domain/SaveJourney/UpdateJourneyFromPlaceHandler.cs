
// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveJourney;

/// <summary>
/// Update journey From Place
/// </summary>
internal sealed class UpdateJourneyFromPlaceHandler : CommandHandler<UpdateJourneyFromPlaceCommand>
{
	private IJourneyRepository Journey { get; init; }

	private ILog<UpdateJourneyFromPlaceHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="log"></param>
	public UpdateJourneyFromPlaceHandler(IJourneyRepository journey, ILog<UpdateJourneyFromPlaceHandler> log) =>
		(Journey, Log) = (journey, log);

	/// <summary>
	/// Update journey From Place belonging to user specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(UpdateJourneyFromPlaceCommand command)
	{
		Log.Vrb("Updating From Place for {Journey}.", command);
		return Journey.UpdateAsync(command);
	}
}
