// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Mileage.Persistence.Common;
using Mileage.Persistence.Tables;
using SimpleMigrations;

namespace Mileage.Persistence.Clients.PostgreSql.Migrations;

/// <summary>
/// Migration 0004: Add Journey table
/// </summary>
[Migration(4, "Add Journey table")]
public sealed class AddJourneyTable : Migration
{
	private string Col(Func<JourneyTable, string> selector) =>
		selector(new());

	/// <summary>
	/// 4: Up
	/// </summary>
	protected override void Up() => Execute($@"
		CREATE TABLE IF NOT EXISTS ""{Constants.Schema}"".""{JourneyTable.TableName}""
		(
			""{Col(j => j.Id)}"" integer NOT NULL GENERATED ALWAYS AS IDENTITY,
			""{Col(j => j.Version)}"" integer NOT NULL DEFAULT 0,
			""{Col(j => j.UserId)}"" integer NOT NULL,
			""{Col(j => j.Date)}"" date NOT NULL,
			""{Col(j => j.CarId)}"" integer NOT NULL,
			""{Col(j => j.StartMiles)}"" integer NOT NULL CHECK (""{Col(j => j.StartMiles)}"" > 0),
			""{Col(j => j.EndMiles)}"" integer CHECK (""{Col(j => j.EndMiles)}"" > 0),
			""{Col(j => j.FromPlaceId)}"" integer,
			""{Col(j => j.ToPlaceIds)}"" text NOT NULL,
			""{Col(j => j.RateId)}"" integer,
			CONSTRAINT ""{Col(j => j.Id)}_Key"" PRIMARY KEY(""{Col(j => j.Id)}"")
		)
		TABLESPACE pg_default
		;
	");

	/// <summary>
	/// 4: Down
	/// </summary>
	protected override void Down() => Execute($@"
		DROP TABLE IF EXISTS ""{Constants.Schema}"".""{JourneyTable.TableName}""
		;
	");
}
