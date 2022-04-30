// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Mileage.Persistence.Common;
using Mileage.Persistence.Tables;
using SimpleMigrations;

namespace Mileage.Persistence.Clients.PostgreSql.Migrations;

/// <summary>
/// Migration 0012: Add IsDisabled column to Rate table
/// </summary>
[Migration(12, "Add IsDisabled column to Rate table")]
public sealed class AddIsDisabledToRateTable : Migration
{
	private string Col(Func<RateTable, string> selector) =>
		selector(new());

	/// <summary>
	/// 12: Up
	/// </summary>
	protected override void Up() => Execute($@"
		ALTER TABLE IF EXISTS {Constants.Schema}.{RateTable.TableName}
		ADD COLUMN {Col(r => r.IsDisabled)} boolean NOT NULL DEFAULT false
		;
	");

	/// <summary>
	/// 12: Down
	/// </summary>
	protected override void Down() => Execute($@"
		ALTER TABLE IF EXISTS {Constants.Schema}.{RateTable.TableName}
		DROP COLUMN IF EXISTS {Col(r => r.IsDisabled)}
		;
	");
}
