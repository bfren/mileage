// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Data;
using Jeebs.Logging;
using Mileage.Persistence;

namespace Mileage.Queries.MigrateToLatest;

/// <summary>
/// Migrate databases
/// </summary>
public sealed class MigrateToLatestHandler : CommandHandler<MigrateToLatestCommand>
{
	private IDb Db { get; init; }

	private Migrator Migrator { get; init; }

	private IAuthDb AuthDb { get; init; }

	private ILog<MigrateToLatestHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="db"></param>
	/// <param name="migrator"></param>
	/// <param name="authDb"></param>
	/// <param name="log"></param>
	public MigrateToLatestHandler(IDb db, Migrator migrator, IAuthDb authDb, ILog<MigrateToLatestHandler> log) =>
		(Db, Migrator, AuthDb, Log) = (db, migrator, authDb, log);

	/// <summary>
	/// Migrate databases to the latest version
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(MigrateToLatestCommand command)
	{
		Log.Dbg("Migrating Authentication database to the latest version.");
		AuthDb.MigrateToLatest();

		Log.Dbg("Migrating Mileage database to the latest version.");
		return F.Some(Migrator.MigrateToLatest(Db.Config.ConnectionString)).AsTask;
	}
}
