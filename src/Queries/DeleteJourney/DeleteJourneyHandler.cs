// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Persistence.Repositories;
using Queries.DeleteJourney.Messages;

namespace Queries.DeleteJourney;

public sealed class DeleteJourneyHandler : IQueryHandler<DeleteJourneyQuery, bool>
{
	private ILog<DeleteJourneyHandler> Log { get; init; }

	private IJourneyRepository Journey { get; init; }

	public DeleteJourneyHandler(IJourneyRepository journey, ILog<DeleteJourneyHandler> log) =>
		(Journey, Log) = (journey, log);

	public Task<Maybe<bool>> HandleAsync(DeleteJourneyQuery query, CancellationToken cancellationToken)
	{
		Log.Dbg("Delete Journey: {Query}", query);
		return Journey
			.QuerySingleAsync<JourneyToDelete>(
				(x => x.Id, Compare.Equal, query.JourneyId),
				(x => x.UserId, Compare.Equal, query.UserId)
			)
			.AuditAsync(
				none: Log.Msg
			)
			.SwitchAsync(
				some: x => Journey.DeleteAsync(x.Id),
				none: _ => F.None<bool>(new JourneyDoesNotExistMsg(query.JourneyId, query.UserId))
			);
	}
}
