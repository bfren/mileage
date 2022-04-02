// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Domain.DeletePlace.Messages;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.DeletePlace;

/// <summary>
/// Delete a place that belongs to a user
/// </summary>
internal sealed class DeletePlaceHandler : CommandHandler<DeletePlaceCommand>
{
	private IPlaceRepository Place { get; init; }

	private ILog<DeletePlaceHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="place"></param>
	/// <param name="log"></param>
	public DeletePlaceHandler(IPlaceRepository place, ILog<DeletePlaceHandler> log) =>
		(Place, Log) = (place, log);

	/// <summary>
	/// Delete the place specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(DeletePlaceCommand command)
	{
		Log.Vrb("Delete Place: {Command}", command);
		return Place
			.StartFluentQuery()
			.Where(x => x.Id, Compare.Equal, command.PlaceId)
			.Where(x => x.UserId, Compare.Equal, command.UserId)
			.QuerySingleAsync<PlaceToDelete>()
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Place.DeleteAsync(x),
				none: _ => F.None<bool>(new PlaceDoesNotExistMsg(command.UserId, command.PlaceId))
			);
	}
}
