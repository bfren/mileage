// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading;
using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Data;
using Jeebs.Logging;
using Mileage.Persistence;

namespace Mileage.Queries.MigrateToLatest;

public sealed class MigrateToLatestHandler : CommandHandler<MigrateToLatestCommand>
{
	private IDb Db { get; init; }

	private Migrator Migrator { get; init; }

	private IAuthDb AuthDb { get; init; }

	private ILog<MigrateToLatestHandler> Log { get; init; }

	public MigrateToLatestHandler(IDb db, Migrator migrator, IAuthDb authDb, ILog<MigrateToLatestHandler> log) =>
		(Db, Migrator, AuthDb, Log) = (db, migrator, authDb, log);

	public override Task<Maybe<bool>> HandleAsync(MigrateToLatestCommand query, CancellationToken cancellationToken)
	{
		Log.Dbg("Migrating Authentication database to the latest version.");
		AuthDb.MigrateToLatest();

		Log.Dbg("Migrating Mileage database to the latest version.");
		return F.Some(Migrator.MigrateToLatest(Db.Config.ConnectionString)).AsTask;
	}
}
