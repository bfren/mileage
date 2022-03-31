// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SavePlace.Internals.CreatePlace;

/// <summary>
/// Create a new place entity
/// </summary>
public sealed class CreatePlaceHandler : QueryHandler<CreatePlaceQuery, PlaceId>
{
	private IPlaceRepository Place { get; init; }

	private ILog<CreatePlaceHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="place"></param>
	/// <param name="log"></param>
	public CreatePlaceHandler(IPlaceRepository place, ILog<CreatePlaceHandler> log) =>
		(Place, Log) = (place, log);

	/// <summary>
	/// Create a new rate from <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<PlaceId>> HandleAsync(CreatePlaceQuery query)
	{
		Log.Vrb("Create Place: {Query}", query);
		return Place
			.CreateAsync(new()
			{
				UserId = query.UserId,
				Description = query.Description,
				Postcode = query.Postcode
			});
	}
}
