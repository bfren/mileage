// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.CheckRateCanBeDeleted;

/// <summary>
/// Check whether or not a rate can be deleted
/// </summary>
internal sealed class CheckRateCanBeDeletedHandler : QueryHandler<CheckRateCanBeDeletedQuery, DeleteOperation>
{
	private IJourneyRepository Journey { get; init; }

	private ISettingsRepository Settings { get; init; }

	private ILog<CheckRateCanBeDeletedHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="settings"></param>
	/// <param name="log"></param>
	public CheckRateCanBeDeletedHandler(IJourneyRepository journey, ISettingsRepository settings, ILog<CheckRateCanBeDeletedHandler> log) =>
		(Journey, Settings, Log) = (journey, settings, log);

	/// <summary>
	/// Check whether or not the rate defined in <paramref name="query"/> can be deleted or disabled
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<DeleteOperation>> HandleAsync(CheckRateCanBeDeletedQuery query) =>
		HandleAsync(query, CheckIsDefaultAsync, CountJourneysWithAsync);

	internal async Task<Maybe<DeleteOperation>> HandleAsync(
		CheckRateCanBeDeletedQuery query,
		CheckIsDefault<RateId> checkIsDefault,
		CountJourneysWith<RateId> countJourneysWith
	)
	{
		Log.Vrb("Checking whether or not Rate {RateId} can be deleted.", query.Id.Value);

		// Check whether or not it is the default rate for the user
		var defaultRateQuery = await checkIsDefault(query.UserId, query.Id);
		if (defaultRateQuery.IsSome(out var isDefaultRate) && isDefaultRate)
		{
			return F.None<DeleteOperation, Messages.RateIsDefaultRateMsg>();
		}
		else if (defaultRateQuery.IsNone(out var reason))
		{
			return F.None<DeleteOperation>(reason);
		}

		// Check whether or not the rate is used in one of the user's journeys
		var journeysWithRateQuery = await countJourneysWith(query.UserId, query.Id);
		return journeysWithRateQuery.Bind(x => x switch
		{
			> 0 =>
				F.Some(DeleteOperation.Disable),

			0 =>
				F.Some(DeleteOperation.Delete),

			_ =>
				F.Some(DeleteOperation.None)
		});
	}

	/// <summary>
	/// Check whether or not <paramref name="rateId"/> is the default rate in a user's settings
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="rateId"></param>
	internal Task<Maybe<bool>> CheckIsDefaultAsync(AuthUserId userId, RateId rateId) =>
		Settings.StartFluentQuery()
			.Where(x => x.UserId, Compare.Equal, userId)
			.ExecuteAsync(x => x.DefaultRateId)
			.BindAsync(x => F.Some(x == rateId));

	/// <summary>
	/// Count the number of journeys using <paramref name="rateId"/>
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="rateId"></param>
	internal Task<Maybe<long>> CountJourneysWithAsync(AuthUserId userId, RateId rateId) =>
		Journey.StartFluentQuery()
			.Where(x => x.UserId, Compare.Equal, userId)
			.Where(x => x.RateId, Compare.Equal, rateId)
			.CountAsync();
}
