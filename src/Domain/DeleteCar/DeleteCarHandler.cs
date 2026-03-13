// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Domain.CheckCarCanBeDeleted;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Repositories;
using Wrap.Caching;

namespace Mileage.Domain.DeleteCar;

/// <summary>
/// Delete a car that belongs to a user
/// </summary>
internal sealed class DeleteCarHandler : CommandHandler<DeleteCarCommand>
{
	private IWrapCache<CarId> Cache { get; init; }

	private ICarRepository Car { get; init; }

	private IDispatcher Dispatcher { get; init; }

	private ILog<DeleteCarHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="cache"></param>
	/// <param name="car"></param>
	/// <param name="dispatcher"></param>
	/// <param name="log"></param>
	public DeleteCarHandler(IWrapCache<CarId> cache, ICarRepository car, IDispatcher dispatcher, ILog<DeleteCarHandler> log) =>
		(Cache, Car, Dispatcher, Log) = (cache, car, dispatcher, log);

	/// <summary>
	/// Delete or disable the car specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Result<bool>> HandleAsync(DeleteCarCommand command) =>
		HandleAsync(command, DeleteOrDisableAsync);

	internal Task<Result<bool>> HandleAsync(DeleteCarCommand command, DeleteOrDisable<CarId> dOrD)
	{
		Log.Vrb("Delete or Disable Car: {Command}", command);
		return Dispatcher
			.SendAsync(new CheckCarCanBeDeletedQuery(command.UserId, command.Id))
			.BindAsync(x => dOrD(command.UserId, command.Id, x))
			.IfOkAsync(x => { if (x) { Cache.RemoveValue(command.Id); } });
	}

	/// <summary>
	/// Peform a delete or disable operation on a car
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="carId"></param>
	/// <param name="operation"></param>
	internal Task<Result<bool>> DeleteOrDisableAsync(AuthUserId userId, CarId carId, DeleteOperation operation) =>
		Car.Fluent()
			.Where(x => x.Id, Compare.Equal, carId)
			.Where(x => x.UserId, Compare.Equal, userId)
			.QuerySingleAsync<CarToDeleteModel>()
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync(
				fOk: async x => operation switch
				{
					DeleteOperation.Delete =>
						await Car.DeleteAsync(x),

					DeleteOperation.Disable =>
						await Car.UpdateAsync(x with { IsDisabled = true }),

					_ =>
						R.Fail("Car {CarId} cannot be deleted.", carId.Value)
							.Ctx(nameof(DeleteCarHandler), nameof(DeleteOrDisableAsync))
				},
				fFail: _ => R.Fail("Car {CarId} does not exist for user {UserId}.", carId.Value, userId.Value)
					.Ctx(nameof(DeleteCarHandler), nameof(DeleteOrDisableAsync))
			);
}
