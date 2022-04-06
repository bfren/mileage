// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Data;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data;
using Jeebs.Logging;

namespace Mileage.Domain.TruncateEverything;

/// <summary>
/// Truncate all tables
/// </summary>
internal sealed class TruncateEverythingHandler : CommandHandler<TruncateEverythingCommand>
{
	private IDb Db { get; init; }

	private ILog<TruncateEverythingHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="db"></param>
	/// <param name="log"></param>
	public TruncateEverythingHandler(IDb db, ILog<TruncateEverythingHandler> log) =>
		(Db, Log) = (db, log);

	/// <summary>
	/// Truncate all tables
	/// </summary>
	/// <param name="command"></param>
	public override async Task<Maybe<bool>> HandleAsync(TruncateEverythingCommand command)
	{
		Log.Inf("Truncating all database tables.");

		Task truncate(string table, IDbTransaction transaction)
		{
			Log.Dbg("Truncating table {Table}.", table);
			return Db.ExecuteAsync($"TRUNCATE TABLE {table};", null, CommandType.Text, transaction);
		}

		using var w = Db.UnitOfWork;
		await truncate("\"auth\".\"User\"", w.Transaction);
		await truncate("\"mileage\".\"Car\"", w.Transaction);
		await truncate("\"mileage\".\"Journey\"", w.Transaction);
		await truncate("\"mileage\".\"Place\"", w.Transaction);
		await truncate("\"mileage\".\"Rate\"", w.Transaction);
		await truncate("\"mileage\".\"Settings\"", w.Transaction);

		return true;
	}
}
