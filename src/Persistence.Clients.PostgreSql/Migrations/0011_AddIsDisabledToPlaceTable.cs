// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Mileage.Persistence.Common;
using Mileage.Persistence.Tables;
using SimpleMigrations;

namespace Mileage.Persistence.Clients.PostgreSql.Migrations;

/// <summary>
/// Migration 0011: Add IsDisabled column to Place table
/// </summary>
[Migration(11, "Add IsDisabled column to Place table")]
public sealed class AddIsDisabledToPlaceTable : Migration
{
	private string Col(Func<PlaceTable, string> selector) =>
		selector(new());

	/// <summary>
	/// 11: Up
	/// </summary>
	protected override void Up() => Execute($@"
		ALTER TABLE IF EXISTS {Constants.Schema}.{PlaceTable.TableName}
		ADD COLUMN {Col(p => p.IsDisabled)} boolean NOT NULL DEFAULT false
		;
	");

	/// <summary>
	/// 11: Down
	/// </summary>
	protected override void Down() => Execute($@"
		ALTER TABLE IF EXISTS {Constants.Schema}.{PlaceTable.TableName}
		DROP COLUMN IF EXISTS {Col(p => p.IsDisabled)}
		;
	");
}
