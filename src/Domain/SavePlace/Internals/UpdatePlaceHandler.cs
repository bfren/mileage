// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SavePlace.Internals;

/// <summary>
/// Update an existing car entity
/// </summary>
internal sealed class UpdatePlaceHandler : CommandHandler<UpdatePlaceCommand>
{
	private IPlaceRepository Place { get; init; }

	private ILog<UpdatePlaceHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="place"></param>
	/// <param name="log"></param>
	public UpdatePlaceHandler(IPlaceRepository place, ILog<UpdatePlaceHandler> log) =>
		(Place, Log) = (place, log);

	/// <summary>
	/// Update a place from <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(UpdatePlaceCommand command)
	{
		Log.Vrb("Update Place: {Command}", command);
		return Place
			.UpdateAsync(new PlaceEntity
			{
				Id = command.PlaceId,
				Version = command.Version,
				Description = command.Description,
				Postcode = command.Postcode
			});
	}
}
