// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using MaybeF.Caching;
using Mileage.Domain.CheckPlaceCanBeDeleted;
using Mileage.Domain.DeletePlace.Messages;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.DeletePlace;

/// <summary>
/// Delete a place that belongs to a user
/// </summary>
internal sealed class DeletePlaceHandler : CommandHandler<DeletePlaceCommand>
{
	private IMaybeCache<PlaceId> Cache { get; init; }

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
	public DeletePlaceHandler(IMaybeCache<PlaceId> cache, IDispatcher dispatcher, IPlaceRepository place, ILog<DeletePlaceHandler> log) =>
		(Cache, Dispatcher, Place, Log) = (cache, dispatcher, place, log);

	/// <summary>
	/// Delete the place specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(DeletePlaceCommand command) =>
		HandleAsync(command, DeleteOrDisableAsync);

	internal Task<Maybe<bool>> HandleAsync(DeletePlaceCommand command, DeleteOrDisable<PlaceId> dOrD)
	{
		Log.Vrb("Delete or Disable Place: {Command}", command);
		return Dispatcher
			.DispatchAsync(new CheckPlaceCanBeDeletedQuery(command.UserId, command.Id))
			.BindAsync(x => dOrD(command.UserId, command.Id, x))
			.IfSomeAsync(x => { if (x) { Cache.RemoveValue(command.Id); } });
	}

	/// <summary>
	/// Peform a delete or disable operation on a place
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="placeId"></param>
	/// <param name="operation"></param>
	internal Task<Maybe<bool>> DeleteOrDisableAsync(AuthUserId userId, PlaceId placeId, DeleteOperation operation) =>
		Place.StartFluentQuery()
			.Where(x => x.Id, Compare.Equal, placeId)
			.Where(x => x.UserId, Compare.Equal, userId)
			.QuerySingleAsync<PlaceToDelete>()
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => operation switch
				{
					DeleteOperation.Delete =>
						Place.DeleteAsync(x),

					DeleteOperation.Disable =>
						Place.UpdateAsync(x with { IsDisabled = true }),

					_ =>
						F.None<bool, PlaceCannotBeDeletedMsg>().AsTask
				},
				none: _ => F.None<bool>(new PlaceDoesNotExistMsg(userId, placeId))
			);
}
