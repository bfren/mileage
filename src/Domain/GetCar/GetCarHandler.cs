// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Jeebs.Messages;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetCar;

internal sealed class GetCarHandler : QueryHandler<GetCarQuery, GetCarModel>
{
	private ICarRepository Car { get; init; }

	private ILog<GetCarHandler> Log { get; init; }

	public GetCarHandler(ICarRepository car, ILog<GetCarHandler> log) =>
		(Car, Log) = (car, log);

	public override Task<Maybe<GetCarModel>> HandleAsync(GetCarQuery query)
	{
		if (query.CarId is null)
		{
			return F.None<GetCarModel, M.CarIdIsNullMsg>().AsTask;
		}

		Log.Vrb("Get car: {Query}.", query);
		return Car
			.StartFluentQuery()
			.Where(x => x.Id, Compare.Equal, query.CarId)
			.Where(x => x.UserId, Compare.Equal, query.UserId)
			.QuerySingleAsync<GetCarModel>();
	}

	/// <summary>Messages</summary>
	public static class M
	{
		public sealed record class CarIdIsNullMsg : Msg;
	}
}
