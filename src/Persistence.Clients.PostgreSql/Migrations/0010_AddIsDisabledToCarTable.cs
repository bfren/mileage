// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Mileage.Persistence.Common;
using Mileage.Persistence.Tables;
using SimpleMigrations;

namespace Mileage.Persistence.Clients.PostgreSql.Migrations;

/// <summary>
/// Migration 0010: Add IsDisabled column to Car table
/// </summary>
[Migration(10, "Add IsDisabled column to Car table")]
public sealed class AddIsDisabledToCarTable : Migration
{
	private string Col(Func<CarTable, string> selector) =>
		selector(new());

	/// <summary>
	/// 10: Up
	/// </summary>
	protected override void Up() => Execute($@"
		ALTER TABLE IF EXISTS {Constants.Schema}.{CarTable.TableName}
		ADD COLUMN {Col(c => c.IsDisabled)} boolean NOT NULL DEFAULT false
		;
	");

	/// <summary>
	/// 10: Down
	/// </summary>
	protected override void Down() => Execute($@"
		ALTER TABLE IF EXISTS {Constants.Schema}.{CarTable.TableName}
		DROP COLUMN IF EXISTS {Col(c => c.IsDisabled)}
		;
	");
}
