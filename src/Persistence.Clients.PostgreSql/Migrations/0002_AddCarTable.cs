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

	/// <summary>
	/// 2: Up
	/// </summary>
	protected override void Up() => Execute($@"
		CREATE TABLE IF NOT EXISTS {Constants.Schema}.{CarTable.TableName}
		(
			{Col(c => c.Id)} integer NOT NULL GENERATED ALWAYS AS IDENTITY,
			{Col(c => c.Version)} integer NOT NULL DEFAULT 0,
			{Col(c => c.UserId)} integer NOT NULL,
			{Col(c => c.Description)} text COLLATE pg_catalog.""en-GB-x-icu"" NOT NULL,
			{Col(c => c.NumberPlate)} text COLLATE pg_catalog.""en-GB-x-icu"",
			CONSTRAINT {Col(c => c.Id)}_key PRIMARY KEY({Col(c => c.Id)})
		)
		TABLESPACE pg_default
		;
	");

	/// <summary>
	/// 2: Down
	/// </summary>
	protected override void Down() => Execute($@"
		DROP TABLE IF EXISTS {Constants.Schema}.{CarTable.TableName}
		;
	");
}
