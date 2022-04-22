// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Domain.DeleteCar.Messages;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.DeleteCar;

/// <summary>
/// Delete a car that belongs to a user
/// </summary>
internal sealed class DeleteCarHandler : CommandHandler<DeleteCarCommand>
{
	private ICarRepository Car { get; init; }

	private ILog<DeleteCarHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="car"></param>
	/// <param name="log"></param>
	public DeleteCarHandler(ICarRepository car, ILog<DeleteCarHandler> log) =>
		(Car, Log) = (car, log);

	/// <summary>
	/// Delete the car specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(DeleteCarCommand command)
	{
		Log.Vrb("Delete Car: {Command}", command);
		return Car
			.StartFluentQuery()
			.Where(x => x.Id, Compare.Equal, command.Id)
			.Where(x => x.UserId, Compare.Equal, command.UserId)
			.QuerySingleAsync<CarToDelete>()
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Car.DeleteAsync(x),
				none: _ => F.None<bool>(new CarDoesNotExistMsg(command.UserId, command.Id))
			);
	}
}
