// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using MaybeF.Caching;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SavePlace.Internals;

/// <summary>
/// Update an existing place entity
/// </summary>
internal sealed class UpdatePlaceHandler : CommandHandler<UpdatePlaceCommand>
{
	private IMaybeCache<PlaceId> Cache { get; init; }

	private IPlaceRepository Place { get; init; }

	private ILog<UpdatePlaceHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="cache"></param>
	/// <param name="place"></param>
	/// <param name="log"></param>
	public UpdatePlaceHandler(IMaybeCache<PlaceId> cache, IPlaceRepository place, ILog<UpdatePlaceHandler> log) =>
		(Cache, Place, Log) = (cache, place, log);

	/// <summary>
	/// Update a place from <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(UpdatePlaceCommand command)
	{
		Log.Vrb("Update Place: {Command}", command);
		return Place
			.UpdateAsync(command)
			.IfSomeAsync(x => { if (x) { Cache.RemoveValue(command.Id); } });
	}
}
