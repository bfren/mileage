// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveJourney;

/// <summary>
/// Update journey Day
/// </summary>
internal sealed class UpdateJourneyDayHandler : CommandHandler<UpdateJourneyDayCommand>
{
	private IJourneyRepository Journey { get; init; }

	private ILog<UpdateJourneyDayHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="log"></param>
	public UpdateJourneyDayHandler(IJourneyRepository journey, ILog<UpdateJourneyDayHandler> log) =>
		(Journey, Log) = (journey, log);

	/// <summary>
	/// Update journey Day belonging to user specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(UpdateJourneyDayCommand command)
	{
		Log.Vrb("Updating Day for {Journey}.", command);
		return Journey.UpdateAsync(command);
	}
}
