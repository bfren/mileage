// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Mileage.Persistence.Common;
using Mileage.Persistence.Tables;
using SimpleMigrations;

namespace Mileage.Persistence.Clients.PostgreSql.Migrations;

/// <summary>
/// Migration 0003: Add Settings table
/// </summary>
[Migration(3, "Add Settings table")]
public sealed class AddSettingsTable : Migration
{
	private string Col(Func<SettingsTable, string> selector) =>
		selector(new());

	protected override void Up() => Execute($@"
		CREATE TABLE IF NOT EXISTS ""{Constants.Schema}"".""{SettingsTable.TableName}""
		(
			""{Col(c => c.Id)}"" integer NOT NULL GENERATED ALWAYS AS IDENTITY,
			""{Col(c => c.Version)}"" integer NOT NULL DEFAULT 0,
			""{Col(c => c.UserId)}"" integer NOT NULL,
			""{Col(c => c.DefaultCarId)}"" integer,
			""{Col(c => c.DefaultFromPlaceId)}"" integer,
			CONSTRAINT ""{Col(c => c.Id)}_Key"" PRIMARY KEY(""{Col(c => c.Id)}"")
		)
		TABLESPACE pg_default
		;
	");

	protected override void Down() => Execute($@"
		DROP TABLE IF EXISTS ""{Constants.Schema}"".""{SettingsTable.TableName}""
		;
	");
}