// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Domain.CheckRateCanBeDeleted;
using Mileage.Domain.DeleteJourney;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Repositories;
using Wrap.Caching;

namespace Mileage.Domain.DeleteRate;

/// <summary>
/// Delete a rate that belongs to a user
/// </summary>
internal sealed class DeleteRateHandler : CommandHandler<DeleteRateCommand>
{
	private IWrapCache<RateId> Cache { get; init; }

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
	public DeleteRateHandler(IWrapCache<RateId> cache, IDispatcher dispatcher, IRateRepository rate, ILog<DeleteRateHandler> log) =>
		(Cache, Dispatcher, Rate, Log) = (cache, dispatcher, rate, log);

	/// <summary>
	/// Delete the rate specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Result<bool>> HandleAsync(DeleteRateCommand command) =>
		HandleAsync(command, DeleteOrDisableAsync);

	internal Task<Result<bool>> HandleAsync(DeleteRateCommand command, DeleteOrDisable<RateId> dOrD)
	{
		Log.Vrb("Delete or Disable Rate: {Command}", command);
		return Dispatcher
			.SendAsync(new CheckRateCanBeDeletedQuery(command.UserId, command.Id))
			.BindAsync(x => dOrD(command.UserId, command.Id, x))
			.IfOkAsync(x => { if (x) { Cache.RemoveValue(command.Id); } });
	}

	/// <summary>
	/// Peform a delete or disable operation on a rate
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="rateId"></param>
	/// <param name="operation"></param>
	internal Task<Result<bool>> DeleteOrDisableAsync(AuthUserId userId, RateId rateId, DeleteOperation operation) =>
		Rate.Fluent()
			.Where(x => x.Id, Compare.Equal, rateId)
			.Where(x => x.UserId, Compare.Equal, userId)
			.QuerySingleAsync<RateToDeleteModel>()
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync(
				fOk: async x => operation switch
				{
					DeleteOperation.Delete =>
						await Rate.DeleteAsync(x),

					DeleteOperation.Disable =>
						await Rate.UpdateAsync(x with { IsDisabled = true }),

					_ =>
						 R.Fail("Rate {RateId} does not exist for user {UserId}.", rateId.Value, userId.Value)
							.Ctx(nameof(DeleteJourneyHandler), nameof(HandleAsync))
				},
				fFail: _ => R.Fail("Rate {RateId} does not exist for user {UserId}.", rateId.Value, userId.Value)
					.Ctx(nameof(DeleteJourneyHandler), nameof(HandleAsync))
			);
}
