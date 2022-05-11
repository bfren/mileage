// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using MaybeF.Caching;
using Mileage.Domain.CheckCarCanBeDeleted;
using Mileage.Domain.DeleteCar.Messages;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.DeleteCar;

/// <summary>
/// Delete a car that belongs to a user
/// </summary>
internal sealed class DeleteCarHandler : CommandHandler<DeleteCarCommand>
{
	private IMaybeCache<CarId> Cache { get; init; }

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
	public DeleteCarHandler(IMaybeCache<CarId> cache, ICarRepository car, IDispatcher dispatcher, ILog<DeleteCarHandler> log) =>
		(Cache, Car, Dispatcher, Log) = (cache, car, dispatcher, log);

	/// <summary>
	/// Delete or disable the car specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(DeleteCarCommand command) =>
		HandleAsync(command, DeleteOrDisableAsync);

	internal Task<Maybe<bool>> HandleAsync(DeleteCarCommand command, DeleteOrDisable<CarId> dOrD)
	{
		Log.Vrb("Delete or Disable Car: {Command}", command);
		return Dispatcher
			.DispatchAsync(new CheckCarCanBeDeletedQuery(command.UserId, command.Id))
			.BindAsync(x => dOrD(command.UserId, command.Id, x))
			.IfSomeAsync(x => { if (x) { Cache.RemoveValue(command.Id); } });
	}

	/// <summary>
	/// Peform a delete or disable operation on a car
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="carId"></param>
	/// <param name="operation"></param>
	internal Task<Maybe<bool>> DeleteOrDisableAsync(AuthUserId userId, CarId carId, DeleteOperation operation) =>
		Car.StartFluentQuery()
			.Where(x => x.Id, Compare.Equal, carId)
			.Where(x => x.UserId, Compare.Equal, userId)
			.QuerySingleAsync<CarToDeleteModel>()
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => operation switch
				{
					DeleteOperation.Delete =>
						Car.DeleteAsync(x),

					DeleteOperation.Disable =>
						Car.UpdateAsync(x with { IsDisabled = true }),

					_ =>
						F.None<bool, CarCannotBeDeletedMsg>().AsTask()
				},
				none: _ => F.None<bool>(new CarDoesNotExistMsg(userId, carId))
			);
}
