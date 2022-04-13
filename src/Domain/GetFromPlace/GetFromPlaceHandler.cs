// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Jeebs.Messages;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetFromPlace;

internal sealed class GetFromPlaceHandler : QueryHandler<GetFromPlaceQuery, GetFromPlaceModel>
{
	private IPlaceRepository Place { get; init; }

	private ILog<GetFromPlaceHandler> Log { get; init; }

	public GetFromPlaceHandler(IPlaceRepository place, ILog<GetFromPlaceHandler> log) =>
		(Place, Log) = (place, log);

	public override Task<Maybe<GetFromPlaceModel>> HandleAsync(GetFromPlaceQuery query)
	{
		if (query.PlaceId is null)
		{
			return F.None<GetFromPlaceModel, M.PlaceIdIsNullMsg>().AsTask;
		}

		Log.Vrb("Get place: {Query}.", query);
		return Place
			.StartFluentQuery()
			.Where(x => x.Id, Compare.Equal, query.PlaceId)
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.QuerySingleAsync<GetFromPlaceModel>();
	}

	/// <summary>Messages</summary>
	public static class M
	{
		public sealed record class PlaceIdIsNullMsg : Msg;
	}
}
