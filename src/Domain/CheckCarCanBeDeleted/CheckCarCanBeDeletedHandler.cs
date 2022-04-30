// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.CheckCarCanBeDeleted;

/// <summary>
/// Check whether or not a car can be deleted
/// </summary>
internal sealed class CheckCarCanBeDeletedHandler : QueryHandler<CheckCarCanBeDeletedQuery, DeleteOperation>
{
	private IJourneyRepository Journey { get; init; }

	private ISettingsRepository Settings { get; init; }

	private ILog<CheckCarCanBeDeletedHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="journey"></param>
	/// <param name="settings"></param>
	/// <param name="log"></param>
	public CheckCarCanBeDeletedHandler(IJourneyRepository journey, ISettingsRepository settings, ILog<CheckCarCanBeDeletedHandler> log) =>
		(Journey, Settings, Log) = (journey, settings, log);

	/// <summary>
	/// Check whether or not the car defined in <paramref name="query"/> can be deleted or disabled
	/// </summary>
	/// <param name="query"></param>
	public override async Task<Maybe<DeleteOperation>> HandleAsync(CheckCarCanBeDeletedQuery query)
	{
		Log.Vrb("Checking whether or not Car {CarId} can be deleted.", query.Id.Value);

		// Check whether or not it is the default car for the user
		var defaultCarQuery = await CheckCarIsDefault(query.UserId, query.Id);
		if (defaultCarQuery.IsSome(out var isDefaultCar) && isDefaultCar)
		{
			return F.None<DeleteOperation, Messages.CarIsDefaultCarMsg>();
		}
		else if (defaultCarQuery.IsNone(out var reason))
		{
			return F.None<DeleteOperation>(reason);
		}

		// Check whether or not the car is used in one of the user's journeys
		var journeysWithCarQuery = await CountJourneysWithCar(query.UserId, query.Id);
		return journeysWithCarQuery.Bind(x => x switch
		{
			> 0 =>
				F.Some(DeleteOperation.Disable),

			0 =>
				F.Some(DeleteOperation.Delete),

			_ =>
				F.Some(DeleteOperation.None)
		});
	}

	/// <summary>
	/// Check whether or not <paramref name="carId"/> is the default car in a user's settings
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="carId"></param>
	internal Task<Maybe<bool>> CheckCarIsDefault(AuthUserId userId, CarId carId) =>
		Settings.StartFluentQuery()
			.Where(x => x.UserId, Compare.Equal, userId)
			.ExecuteAsync(x => x.DefaultCarId)
			.BindAsync(x => F.Some(x == carId));

	/// <summary>
	/// Count the number of journeys using <paramref name="carId"/>
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="carId"></param>
	internal Task<Maybe<long>> CountJourneysWithCar(AuthUserId userId, CarId carId) =>
		Journey.StartFluentQuery()
			.Where(x => x.UserId, Compare.Equal, userId)
			.Where(x => x.CarId, Compare.Equal, carId)
			.CountAsync();
}
