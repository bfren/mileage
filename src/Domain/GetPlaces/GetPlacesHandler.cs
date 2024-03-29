// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Collections.Generic;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetPlaces;

/// <summary>
/// Get places
/// </summary>
internal sealed class GetPlacesHandler : QueryHandler<GetPlacesQuery, IEnumerable<PlacesModel>>
{
	private IPlaceRepository Place { get; init; }

	private ILog<GetPlacesHandler> Log { get; init; }

	private readonly bool[] trueAndFalse = [true, false];

	private readonly bool[] falseOnly = [false];

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="place"></param>
	/// <param name="log"></param>
	public GetPlacesHandler(IPlaceRepository place, ILog<GetPlacesHandler> log) =>
		(Place, Log) = (place, log);

	/// <summary>
	/// Get places for the specified user, sorted by description
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<IEnumerable<PlacesModel>>> HandleAsync(GetPlacesQuery query)
	{
		if (query.UserId is null || query.UserId.Value == 0)
		{
			return F.None<IEnumerable<PlacesModel>, Messages.UserIdIsNullMsg>().AsTask();
		}

		Log.Vrb("Get Places for {User}.", query.UserId);
		return Place
			.StartFluentQuery()
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.WhereIn(x => x.IsDisabled, query.IncludeDisabled ? trueAndFalse : falseOnly)
			.Sort(x => x.Description, SortOrder.Ascending)
			.QueryAsync<PlacesModel>();
	}
}
