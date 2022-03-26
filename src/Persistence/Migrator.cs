// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using System.Data.Common;
using System.Reflection;
using Mileage.Persistence.Common;
using SimpleMigrations;
using SimpleMigrations.Console;
using SimpleMigrations.DatabaseProvider;

namespace Mileage.Persistence;

/// <summary>
/// Enables simple database migrations
/// </summary>
public abstract class Migrator
{
	/// <summary>
	/// Migrate to the latest database schema
	/// </summary>
	/// <param name="connectionString">Use this to create a <see cref="DbConnection"/> object</param>
	public bool MigrateToLatest(string connectionString) =>
		MigrateTo(connectionString, null);

	/// <summary>
	/// Migrate to the specified <paramref name="version"/> (or the latest if it's null)
	/// </summary>
	/// <param name="connectionString">Use this to create a <see cref="DbConnection"/> object</param>
	/// <param name="version">The version to migrate to (or null for 'latest')</param>
	public abstract bool MigrateTo(string connectionString, long? version);

	/// <inheritdoc cref="MigrateTo(string, long?)"/>
	/// <param name="dbType">DbType</param>
	/// <param name="dbConnection">DbConnection</param>
	/// <param name="migrationsAssembly">The assembly from which to load migrations</param>
	/// <param name="version">The version to migrate to - if null will migrate to the latest version</param>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	protected static bool MigrateTo(ClientType dbType, DbConnection dbConnection, Assembly migrationsAssembly, long? version)
	{
		// Get the correct provider
		var provider = dbType switch
		{
			ClientType.PostgreSql =>
				new PostgresqlDatabaseProvider(dbConnection) { SchemaName = Constants.Schema, TableName = @"""VersionInfo""" },

			_ =>
				throw new ArgumentOutOfRangeException(nameof(dbType), dbType, "Unknown database client type.")
		};

		// Create the migrator
		var migrator = new SimpleMigrator(migrationsAssembly, provider, new ConsoleLogger());

		// Get all the migrations
		migrator.Load();

		// Migrate to specific version, or the latest version
		if (version is long specificVersion)
		{
			migrator.MigrateTo(specificVersion);
		}
		else
		{
			migrator.MigrateToLatest();
		}

		// Ensure the migration succeeded
		return migrator.LatestMigration.Version == version;
	}
}
