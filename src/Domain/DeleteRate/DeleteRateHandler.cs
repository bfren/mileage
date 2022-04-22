// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Domain.DeleteRate.Messages;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.DeleteRate;

/// <summary>
/// Delete a rate that belongs to a user
/// </summary>
internal sealed class DeleteRateHandler : CommandHandler<DeleteRateCommand>
{
	private IRateRepository Rate { get; init; }

	private ILog<DeleteRateHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="rate"></param>
	/// <param name="log"></param>
	public DeleteRateHandler(IRateRepository rate, ILog<DeleteRateHandler> log) =>
		(Rate, Log) = (rate, log);

	/// <summary>
	/// Delete the rate specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(DeleteRateCommand command)
	{
		Log.Vrb("Delete Rate: {Command}", command);
		return Rate
			.StartFluentQuery()
			.Where(x => x.Id, Compare.Equal, command.Id)
			.Where(x => x.UserId, Compare.Equal, command.UserId)
			.QuerySingleAsync<RateToDelete>()
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Rate.DeleteAsync(x),
				none: _ => F.None<bool>(new RateDoesNotExistMsg(command.UserId, command.Id))
			);
	}
}
