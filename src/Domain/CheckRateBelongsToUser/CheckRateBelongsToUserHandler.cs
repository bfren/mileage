// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.CheckRateBelongsToUser;

/// <summary>
/// Check a rate belongs to a user
/// </summary>
internal sealed class CheckRateBelongsToUserHandler : QueryHandler<CheckRateBelongsToUserQuery, bool>
{
	private IRateRepository Rate { get; init; }

	private ILog<CheckRateBelongsToUserHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="rate"></param>
	/// <param name="log"></param>
	public CheckRateBelongsToUserHandler(IRateRepository rate, ILog<CheckRateBelongsToUserHandler> log) =>
		(Rate, Log) = (rate, log);

	/// <summary>
	/// Returns true if the rate belongs to the user defined by <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<bool>> HandleAsync(CheckRateBelongsToUserQuery query)
	{
		Log.Vrb("Checking rate {RateId} belongs to user {UserId}.", query.RateId.Value, query.UserId.Value);
		return Rate
			.StartFluentQuery()
			.Where(c => c.Id, Compare.Equal, query.RateId)
			.Where(c => c.UserId, Compare.Equal, query.UserId)
			.QuerySingleAsync<RateEntity>()
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: _ => F.True,
				none: _ => F.False
			);
	}
}
