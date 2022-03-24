// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Data.Common;

namespace Mileage.Persistence.Clients.PostgreSql;

/// <summary>
/// PostgreSQL database client migrator
/// </summary>
public sealed class PostgreSqlMigrator : Migrator
{
	/// <inheritdoc/>
	public override bool MigrateTo(DbConnection dbConnection, long? version) =>
		MigrateTo(ClientType.PostgreSql, dbConnection, typeof(PostgreSqlMigrator).Assembly, version);
}
