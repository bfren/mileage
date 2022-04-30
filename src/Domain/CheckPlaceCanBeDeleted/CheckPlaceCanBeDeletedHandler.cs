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
using Mileage.Persistence.Tables;

namespace Mileage.Domain.CheckPlaceCanBeDeleted;

/// <summary>
/// Check whether or not a car can be deleted
/// </summary>
internal sealed class CheckPlaceCanBeDeletedHandler : QueryHandler<CheckPlaceCanBeDeletedQuery, DeleteOperation>
{
	private IJourneyRepository Journey { get; init; }

	private ISettingsRepository Settings { get; init; }

	private ILog<CheckPlaceCanBeDeletedHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="settings"></param>
	/// <param name="log"></param>
	public CheckPlaceCanBeDeletedHandler(IJourneyRepository journey, ISettingsRepository settings, ILog<CheckPlaceCanBeDeletedHandler> log) =>
		(Journey, Settings, Log) = (journey, settings, log);

	/// <summary>
	/// Check whether or not the car defined in <paramref name="query"/> can be deleted or disabled
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<DeleteOperation>> HandleAsync(CheckPlaceCanBeDeletedQuery query) =>
		HandleAsync(query, CheckIsDefaultAsync, CountJourneysWithAsync);

	internal async Task<Maybe<DeleteOperation>> HandleAsync(
		CheckPlaceCanBeDeletedQuery query,
		CheckIsDefault<PlaceId> checkIsDefault,
		CountJourneysWith<PlaceId> countJourneysWith
	)
	{
		Log.Vrb("Checking whether or not Place {PlaceId} can be deleted.", query.Id.Value);

		// Check whether or not it is the default from place for the user
		var defaultFromPlaceQuery = await checkIsDefault(query.UserId, query.Id);
		if (defaultFromPlaceQuery.IsSome(out var isDefaultFromPlace) && isDefaultFromPlace)
		{
			return F.None<DeleteOperation, Messages.PlaceIsDefaultFromPlaceMsg>();
		}
		else if (defaultFromPlaceQuery.IsNone(out var reason))
		{
			return F.None<DeleteOperation>(reason);
		}

		// Check whether or not the place is used in one of the user's journeys
		var journeysWithPlaceQuery = await countJourneysWith(query.UserId, query.Id);
		return journeysWithPlaceQuery.Bind(x => x switch
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
	/// Check whether or not <paramref name="placeId"/> is the default from place in a user's settings
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="placeId"></param>
	internal Task<Maybe<bool>> CheckIsDefaultAsync(AuthUserId userId, PlaceId placeId) =>
		Settings.StartFluentQuery()
			.Where(x => x.UserId, Compare.Equal, userId)
			.ExecuteAsync(x => x.DefaultFromPlaceId)
			.BindAsync(x => F.Some(x == placeId));

	/// <summary>
	/// Count the number of journeys using <paramref name="placeId"/>
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="placeId"></param>
	internal Task<Maybe<long>> CountJourneysWithAsync(AuthUserId userId, PlaceId placeId)
	{
		var j = new JourneyTable();
		return Journey.StartFluentQuery()
			.Where(x => x.UserId, Compare.Equal, userId)
			.Where($"({j.FromPlaceId} = @{nameof(placeId)} OR {j.ToPlaceIds} ? @{nameof(placeId)}::text)", new { placeId })
			.CountAsync();
	}
}
