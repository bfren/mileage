// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Mileage.Persistence.Common;
using Mileage.Persistence.Tables;
using SimpleMigrations;

namespace Mileage.Persistence.Clients.PostgreSql.Migrations;

/// <summary>
/// Migration 0002: Add Car table
/// </summary>
[Migration(2, "Add Car table")]
public sealed class AddCarTable : Migration
{
	private string Col(Func<CarTable, string> selector) =>
		selector(new());

	protected override void Up() => Execute($@"
		CREATE TABLE IF NOT EXISTS ""{Constants.Schema}"".""{CarTable.TableName}""
		(
			""{Col(c => c.Id)}"" integer NOT NULL GENERATED ALWAYS AS IDENTITY,
			""{Col(c => c.Version)}"" integer NOT NULL DEFAULT 0,
			""{Col(c => c.UserId)}"" integer NOT NULL,
			""{Col(c => c.Description)}"" character(32) COLLATE pg_catalog.default NOT NULL,
			CONSTRAINT ""{Col(c => c.Id)}_Key"" PRIMARY KEY(""{Col(c => c.Id)}"")
		)
		TABLESPACE pg_default
		;
	");

	protected override void Down() => Execute($@"
		DROP TABLE IF EXISTS ""{Constants.Schema}"".""{CarTable.TableName}""
		;
	");
}
