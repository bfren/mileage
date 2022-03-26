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
public sealed class DeleteJourneyHandler : QueryHandler<DeleteJourneyQuery, bool>
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
	/// Delete the journey specified in <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<bool>> HandleAsync(DeleteJourneyQuery query)
	{
		Log.Vrb("Delete Journey: {Query}", query);
		return Journey
			.StartFluentQuery()
			.Where(
				x => x.Id, Compare.Equal, query.JourneyId
			)
			.Where(
				x => x.UserId, Compare.Equal, query.UserId
			)
			.QuerySingleAsync<JourneyToDelete>()
			.AuditAsync(
				none: Log.Msg
			)
			.SwitchAsync(
				some: x => Journey.DeleteAsync(x),
				none: _ => F.None<bool>(new JourneyDoesNotExistMsg(query.JourneyId, query.UserId))
			);
	}
}
