// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Domain.CheckPlaceCanBeDeleted;
using Mileage.Domain.DeleteJourney;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Repositories;
using Wrap.Caching;

namespace Mileage.Domain.DeletePlace;

/// <summary>
/// Delete a place that belongs to a user
/// </summary>
internal sealed class DeletePlaceHandler : CommandHandler<DeletePlaceCommand>
{
	private IWrapCache<PlaceId> Cache { get; init; }

	private IDispatcher Dispatcher { get; init; }

	private IPlaceRepository Place { get; init; }

	private ILog<DeletePlaceHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="cache"></param>
	/// <param name="dispatcher"></param>
	/// <param name="place"></param>
	/// <param name="log"></param>
	public DeletePlaceHandler(IWrapCache<PlaceId> cache, IDispatcher dispatcher, IPlaceRepository place, ILog<DeletePlaceHandler> log) =>
		(Cache, Dispatcher, Place, Log) = (cache, dispatcher, place, log);

	/// <summary>
	/// Delete the place specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Result<bool>> HandleAsync(DeletePlaceCommand command) =>
		HandleAsync(command, DeleteOrDisableAsync);

	internal Task<Result<bool>> HandleAsync(DeletePlaceCommand command, DeleteOrDisable<PlaceId> dOrD)
	{
		Log.Vrb("Delete or Disable Place: {Command}", command);
		return Dispatcher
			.SendAsync(new CheckPlaceCanBeDeletedQuery(command.UserId, command.Id))
			.BindAsync(x => dOrD(command.UserId, command.Id, x))
			.IfOkAsync(x => { if (x) { Cache.RemoveValue(command.Id); } });
	}

	/// <summary>
	/// Peform a delete or disable operation on a place
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="placeId"></param>
	/// <param name="operation"></param>
	internal Task<Result<bool>> DeleteOrDisableAsync(AuthUserId userId, PlaceId placeId, DeleteOperation operation) =>
		Place.Fluent()
			.Where(x => x.Id, Compare.Equal, placeId)
			.Where(x => x.UserId, Compare.Equal, userId)
			.QuerySingleAsync<PlaceToDeleteModel>()
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync(
				fOk: async x => operation switch
				{
					DeleteOperation.Delete =>
						await Place.DeleteAsync(x),

					DeleteOperation.Disable =>
						await Place.UpdateAsync(x with { IsDisabled = true }),

					_ =>
						R.Fail("Place {PlaceId} does not exist for user {UserId}.", placeId.Value, userId.Value)
							.Ctx(nameof(DeleteJourneyHandler), nameof(HandleAsync))
				},
				fFail: _ => R.Fail("Place {PlaceId} does not exist for user {UserId}.", placeId.Value, userId.Value)
					.Ctx(nameof(DeleteJourneyHandler), nameof(HandleAsync))
			);
}
