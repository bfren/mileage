// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common;
using SimpleMigrations;

namespace Mileage.Persistence.Clients.PostgreSql.Migrations;

/// <summary>
/// Migration 0001: Add schema
/// </summary>
[Migration(1, "Add Mileage schema")]
public sealed class AddSchema : Migration
{
	/// <summary>
	/// 1: Up
	/// </summary>
	protected override void Up() => Execute($@"
		CREATE SCHEMA IF NOT EXISTS ""{Constants.Schema}""
		;
	");

	/// <summary>
	/// 1: Down
	/// </summary>
	protected override void Down() { }
}
