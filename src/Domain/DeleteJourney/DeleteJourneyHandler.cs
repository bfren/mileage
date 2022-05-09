// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Domain.DeleteJourney.Messages;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.DeleteJourney;

/// <summary>
/// Delete a journey that belongs to a user
/// </summary>
internal sealed class DeleteJourneyHandler : CommandHandler<DeleteJourneyCommand>
{
	private ILog<DeleteJourneyHandler> Log { get; init; }

	private IJourneyRepository Journey { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="log"></param>
	public DeleteJourneyHandler(IJourneyRepository journey, ILog<DeleteJourneyHandler> log) =>
		(Journey, Log) = (journey, log);

	/// <summary>
	/// Delete the journey specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(DeleteJourneyCommand command)
	{
		Log.Vrb("Delete Journey: {Command}", command);
		return Journey
			.StartFluentQuery()
			.Where(x => x.Id, Compare.Equal, command.Id)
			.Where(x => x.UserId, Compare.Equal, command.UserId)
			.QuerySingleAsync<JourneyToDeleteModel>()
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Journey.DeleteAsync(x),
				none: _ => F.None<bool>(new JourneyDoesNotExistMsg(command.UserId, command.Id))
			);
	}
}
