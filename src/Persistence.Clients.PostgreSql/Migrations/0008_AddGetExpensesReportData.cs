// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using SimpleMigrations;

namespace Mileage.Persistence.Clients.PostgreSql.Migrations;

/// <summary>
/// Migration 0008: retained for historical reasons - see
/// <see cref="AddGetExpensesReportDataWithCarNumberPlate"/>
/// </summary>
[Migration(8, "Unused - see migration 9")]
public sealed class AddGetExpensesReportData : Migration
{
	/// <summary>
	/// 8: Up
	/// </summary>
	protected override void Up() =>
		Execute("SELECT 1;");

	/// <summary>
	/// 8: Down
	/// </summary>
	protected override void Down() =>
		Execute("SELECT 1;");
}
