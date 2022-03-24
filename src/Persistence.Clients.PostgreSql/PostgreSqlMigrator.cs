// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Npgsql;

namespace Mileage.Persistence.Clients.PostgreSql;

/// <summary>
/// PostgreSQL database client migrator
/// </summary>
public sealed class PostgreSqlMigrator : Migrator
{
	/// <inheritdoc/>
	public override bool MigrateTo(string connectionString, long? version)
	{
		using var dbConnection = new NpgsqlConnection(connectionString);
		return MigrateTo(ClientType.PostgreSql, dbConnection, typeof(PostgreSqlMigrator).Assembly, version);
	}
}
