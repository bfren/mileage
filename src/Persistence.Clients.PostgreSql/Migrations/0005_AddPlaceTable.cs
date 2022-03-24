// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Mileage.Persistence.Common;
using Mileage.Persistence.Tables;
using SimpleMigrations;

namespace Mileage.Persistence.Clients.PostgreSql.Migrations;

/// <summary>
/// Migration 0005: Add Place table
/// </summary>
[Migration(5, "Add Place table")]
public sealed class AddPlaceTable : Migration
{
	private string Col(Func<PlaceTable, string> selector) =>
		selector(new());

	protected override void Up() => Execute($@"
		CREATE TABLE IF NOT EXISTS ""{Constants.Schema}"".""{PlaceTable.TableName}""
		(
			""{Col(p => p.Id)}"" integer NOT NULL GENERATED ALWAYS AS IDENTITY,
			""{Col(p => p.Version)}"" integer NOT NULL DEFAULT 0,
			""{Col(p => p.UserId)}"" integer NOT NULL,
			""{Col(p => p.Description)}"" character(64) NOT NULL,
			""{Col(p => p.Postcode)}"" character(8),
			CONSTRAINT ""{Col(p => p.Id)}_Key"" PRIMARY KEY(""{Col(p => p.Id)}"")
		)
		TABLESPACE pg_default
		;
	");

	protected override void Down() => Execute($@"
		DROP TABLE IF EXISTS ""{Constants.Schema}"".""{PlaceTable.TableName}""
		;
	");
}
