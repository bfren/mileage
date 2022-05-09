// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using MaybeF.Caching;
using Mileage.Domain.CheckRateCanBeDeleted;
using Mileage.Domain.DeleteRate.Messages;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.DeleteRate;

/// <summary>
/// Delete a rate that belongs to a user
/// </summary>
internal sealed class DeleteRateHandler : CommandHandler<DeleteRateCommand>
{
	private IMaybeCache<RateId> Cache { get; init; }

	private IDispatcher Dispatcher { get; init; }

	private IRateRepository Rate { get; init; }

	private ILog<DeleteRateHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="cache"></param>
	/// <param name="dispatcher"></param>
	/// <param name="rate"></param>
	/// <param name="log"></param>
	public DeleteRateHandler(IMaybeCache<RateId> cache, IDispatcher dispatcher, IRateRepository rate, ILog<DeleteRateHandler> log) =>
		(Cache, Dispatcher, Rate, Log) = (cache, dispatcher, rate, log);

	/// <summary>
	/// Delete the rate specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(DeleteRateCommand command) =>
		HandleAsync(command, DeleteOrDisableAsync);

	internal Task<Maybe<bool>> HandleAsync(DeleteRateCommand command, DeleteOrDisable<RateId> dOrD)
	{
		Log.Vrb("Delete or Disable Rate: {Command}", command);
		return Dispatcher
			.DispatchAsync(new CheckRateCanBeDeletedQuery(command.UserId, command.Id))
			.BindAsync(x => dOrD(command.UserId, command.Id, x))
			.IfSomeAsync(x => { if (x) { Cache.RemoveValue(command.Id); } });
	}

	/// <summary>
	/// Peform a delete or disable operation on a rate
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="rateId"></param>
	/// <param name="operation"></param>
	internal Task<Maybe<bool>> DeleteOrDisableAsync(AuthUserId userId, RateId rateId, DeleteOperation operation) =>
		Rate.StartFluentQuery()
			.Where(x => x.Id, Compare.Equal, rateId)
			.Where(x => x.UserId, Compare.Equal, userId)
			.QuerySingleAsync<RateToDeleteModel>()
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => operation switch
				{
					DeleteOperation.Delete =>
						Rate.DeleteAsync(x),

					DeleteOperation.Disable =>
						Rate.UpdateAsync(x with { IsDisabled = true }),

					_ =>
						F.None<bool, RateCannotBeDeletedMsg>().AsTask
				},
				none: _ => F.None<bool>(new RateDoesNotExistMsg(userId, rateId))
			);
}
