// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Mileage.Persistence.Common;
using Mileage.Persistence.Tables;
using SimpleMigrations;

namespace Mileage.Persistence.Clients.PostgreSql.Migrations;

/// <summary>
/// Migration 0006: Add Rate table
/// </summary>
[Migration(6, "Add Rate table")]
public sealed class AddRateTable : Migration
{
	private string Col(Func<RateTable, string> selector) =>
		selector(new());

	protected override void Up() => Execute($@"
		CREATE TABLE IF NOT EXISTS ""{Constants.Schema}"".""{RateTable.TableName}""
		(
			""{Col(r => r.Id)}"" integer NOT NULL GENERATED ALWAYS AS IDENTITY,
			""{Col(r => r.Version)}"" integer NOT NULL DEFAULT 0,
			""{Col(r => r.UserId)}"" integer NOT NULL,
			""{Col(r => r.AmountPerMileGBP)}"" numeric(4,2) NOT NULL CHECK (""{Col(r => r.AmountPerMileGBP)}"" > 0),
			CONSTRAINT ""{Col(r => r.Id)}_Key"" PRIMARY KEY(""{Col(r => r.Id)}"")
		)
		TABLESPACE pg_default
		;
	");

	protected override void Down() => Execute($@"
		DROP TABLE IF EXISTS ""{Constants.Schema}"".""{RateTable.TableName}""
		;
	");
}
