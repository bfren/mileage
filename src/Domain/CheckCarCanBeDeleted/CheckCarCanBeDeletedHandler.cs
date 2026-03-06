// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Domain.CheckPlaceCanBeDeleted;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.Ids;
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
	public override Task<Result<DeleteOperation>> HandleAsync(CheckCarCanBeDeletedQuery query) =>
		HandleAsync(query, CheckIsDefaultAsync, CountJourneysWithAsync);

	internal async Task<Result<DeleteOperation>> HandleAsync(
		CheckCarCanBeDeletedQuery query,
		CheckIsDefault<CarId> checkIsDefault,
		CountJourneysWith<CarId> countJourneysWith
	)
	{
		Log.Vrb("Checking whether or not Car {CarId} can be deleted.", query.Id.Value);

		// Check whether or not it is the default car for the user
		var defaultCarQuery = await checkIsDefault(query.UserId, query.Id);
		if (defaultCarQuery.Unsafe().TryOk(out var isDefaultCar) && isDefaultCar)
		{
			return R.Fail("Car {CarId} cannot be deleted as it is the default for user {UserId}.", query.Id.Value, query.UserId.Value)
				.Ctx(nameof(CheckCarCanBeDeletedHandler), nameof(HandleAsync));
		}
		else if (defaultCarQuery.Unsafe().TryFailure(out var reason))
		{
			return R.Fail(reason)
				.Ctx(nameof(CheckPlaceCanBeDeletedHandler), nameof(HandleAsync));
		}

		// Check whether or not the car is used in one of the user's journeys
		var journeysWithCarQuery = await countJourneysWith(query.UserId, query.Id);
		return journeysWithCarQuery.Bind(x => x switch
		{
			> 0 =>
				R.Wrap(DeleteOperation.Disable),

			0 =>
				R.Wrap(DeleteOperation.Delete),

			_ =>
				R.Wrap(DeleteOperation.None)
		});
	}

	/// <summary>
	/// Check whether or not <paramref name="carId"/> is the default car in a user's settings
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="carId"></param>
	internal Task<Result<bool>> CheckIsDefaultAsync(AuthUserId userId, CarId carId) =>
		Settings.Fluent()
			.Where(x => x.UserId, Compare.Equal, userId)
			.ExecuteAsync(x => x.DefaultCarId)
			.MapAsync(x => x == carId);

	/// <summary>
	/// Count the number of journeys using <paramref name="carId"/>
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="carId"></param>
	internal Task<Result<long>> CountJourneysWithAsync(AuthUserId userId, CarId carId) =>
		Journey.Fluent()
			.Where(x => x.UserId, Compare.Equal, userId)
			.Where(x => x.CarId, Compare.Equal, carId)
			.CountAsync();
}
