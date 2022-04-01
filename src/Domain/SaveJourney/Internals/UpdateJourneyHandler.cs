// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using System.Threading.Tasks;
using Jeebs.Collections;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveJourney.Internals;

/// <summary>
/// Update an existing mileage entity
/// </summary>
internal sealed class UpdateJourneyHandler : CommandHandler<UpdateJourneyCommand>
{
	private IJourneyRepository Journey { get; init; }

	private ILog<UpdateJourneyHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="log"></param>
	public UpdateJourneyHandler(IJourneyRepository journey, ILog<UpdateJourneyHandler> log) =>
		(Journey, Log) = (journey, log);

	/// <summary>
	/// Update an existing journey from <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(UpdateJourneyCommand command)
	{
		Log.Vrb("Update Journey: {Command}", command);
		return Journey
			.UpdateAsync(new JourneyEntity()
			{
				Id = command.JourneyId,
				Version = command.Version,
				UserId = command.UserId,
				Date = command.Date.ToDateTime(TimeOnly.MinValue),
				CarId = command.CarId,
				StartMiles = (int)command.StartMiles,
				EndMiles = (int?)command.EndMiles,
				FromPlaceId = command.FromPlaceId,
				ToPlaceIds = ImmutableList.Create(args: command.ToPlaceIds),
				RateId = command.RateId
			});
	}
}
