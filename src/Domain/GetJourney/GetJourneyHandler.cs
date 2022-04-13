// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Jeebs.Messages;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetJourney;

internal sealed class GetJourneyHandler : QueryHandler<GetJourneyQuery, GetJourneyModel>
{
	private IJourneyRepository Journey { get; init; }

	private ILog<GetJourneyHandler> Log { get; init; }

	public GetJourneyHandler(IJourneyRepository journey, ILog<GetJourneyHandler> log) =>
		(Journey, Log) = (journey, log);

	public override Task<Maybe<GetJourneyModel>> HandleAsync(GetJourneyQuery query)
	{
		if (query.JourneyId is null)
		{
			return F.None<GetJourneyModel, M.JourneyIdIsNullMsg>().AsTask;
		}

		Log.Vrb("Get journey: {Query}.", query);
		return Journey
			.StartFluentQuery()
			.Where(x => x.Id, Compare.Equal, query.JourneyId)
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.QuerySingleAsync<GetJourneyModel>();
	}

	/// <summary>Messages</summary>
	public static class M
	{
		public sealed record class JourneyIdIsNullMsg : Msg;
	}
}
