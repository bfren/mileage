// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using System.Linq;
using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Domain.CheckCarBelongsToUser;
using Mileage.Domain.CheckPlacesBelongToUser;
using Mileage.Domain.CheckRateBelongsToUser;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveJourney;

/// <summary>
/// Save a journey - create if it doesn't exist, or update if it does
/// </summary>
internal sealed class SaveJourneyHandler : QueryHandler<SaveJourneyQuery, JourneyId>
{
	private IDispatcher Dispatcher { get; init; }

	private IJourneyRepository Journey { get; init; }

	private ILog<SaveJourneyHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="dispatcher"></param>
	/// <param name="journey"></param>
	/// <param name="log"></param>
	public SaveJourneyHandler(IDispatcher dispatcher, IJourneyRepository journey, ILog<SaveJourneyHandler> log) =>
		(Dispatcher, Journey, Log) = (dispatcher, journey, log);

	/// <summary>
	/// Save the journey belonging to user specified in <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	/// <exception cref="NotImplementedException"></exception>
	public override async Task<Maybe<JourneyId>> HandleAsync(SaveJourneyQuery query)
	{
		// Ensure the car, from place, to places, and rate belong to the user (or that the rate is null)
		var carBelongsToUser = await CheckCarBelongsToUser(query.UserId, query.CarId);
		var fromBelongsToUser = await CheckPlacesBelongToUser(query.UserId, query.FromPlaceId);
		var toBelongToUser = await CheckPlacesBelongToUser(query.UserId, query.ToPlaceIds);
		var rateBelongsToUser = await CheckRateBelongsToUser(query.UserId, query.RateId);

		// If checks have failed, return with failure message
		if (!carBelongsToUser || !fromBelongsToUser || !toBelongToUser || !rateBelongsToUser)
		{
			return F.None<JourneyId, Messages.SaveJourneyCheckFailedMsg>();
		}

		// Add or update Journey
		return await Journey
			.StartFluentQuery()
			.Where(s => s.Id, Compare.Equal, query.JourneyId)
			.Where(s => s.UserId, Compare.Equal, query.UserId)
			.QuerySingleAsync<JourneyEntity>()
			.SwitchAsync(
				some: x => Dispatcher
					.DispatchAsync(new Internals.UpdateJourneyCommand(x.Id, query))
					.BindAsync(_ => F.Some(x.Id)),
				none: () => Dispatcher
					.DispatchAsync(new Internals.CreateJourneyQuery(query))
			);
	}

	/// <summary>
	/// Returns true if <paramref name="carId"/> belongs to <paramref name="userId"/>
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="carId"></param>
	internal Task<bool> CheckCarBelongsToUser(AuthUserId userId, CarId carId) =>
		Dispatcher
			.DispatchAsync(new CheckCarBelongsToUserQuery(userId, carId))
			.IfSomeAsync(
				x => { if (!x) { Log.Dbg("Car {CarId} does not belong to user {UserId}.", carId.Value, userId.Value); } }
			)
			.IsTrueAsync();

	/// <summary>
	/// Returns true if <paramref name="placeIds"/> belongs to <paramref name="userId"/>
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="placeIds"></param>
	internal Task<bool> CheckPlacesBelongToUser(AuthUserId userId, params PlaceId[]? placeIds) =>
		placeIds switch
		{
			PlaceId[] p =>
				Dispatcher
					.DispatchAsync(new CheckPlacesBelongToUserQuery(userId, p))
					.IfSomeAsync(
						x => { if (!x) { Log.Dbg("Places {PlaceIds} do not all belong to user {UserId}.", p.Select(p => p.Value), userId.Value); } }
					)
					.IsTrueAsync(),

			_ =>
				Task.FromResult(true)
		};

	/// <summary>
	/// Returns true if <paramref name="rateId"/> belongs to <paramref name="userId"/>
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="rateId"></param>
	internal Task<bool> CheckRateBelongsToUser(AuthUserId userId, RateId? rateId) =>
		rateId switch
		{
			RateId r =>
				Dispatcher
					.DispatchAsync(new CheckRateBelongsToUserQuery(userId, rateId))
					.IfSomeAsync(
						x => { if (!x) { Log.Dbg("Rate {RateId} does not belong to user {UserId}.", rateId.Value, userId.Value); } }
					)
					.IsTrueAsync(),

			_ =>
				Task.FromResult(true)
		};
}
