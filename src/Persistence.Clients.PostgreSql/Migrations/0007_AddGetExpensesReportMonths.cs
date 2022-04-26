// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Mileage.Persistence.Common;
using Mileage.Persistence.Tables;
using SimpleMigrations;

namespace Mileage.Persistence.Clients.PostgreSql.Migrations;

/// <summary>
/// Migration 0007: Add function to get expenses report months
/// </summary>
[Migration(7, "Add function to get expenses report months")]
public sealed class AddGetExpensesReportMonths : Migration
{
	private string Col(Func<JourneyTable, string> selector) =>
		$"{Constants.Schema}.{JourneyTable.TableName}.{selector(new())}";

	/// <summary>
	/// 7: Up
	/// </summary>
	protected override void Up() => Execute($@"
		CREATE OR REPLACE FUNCTION {Constants.Schema}.{Constants.Functions.GetExpensesReportRecentMonths}(
			user_id bigint,
			months integer DEFAULT 6)
			RETURNS TABLE(year numeric, month numeric) 
			LANGUAGE 'plpgsql'
			COST 100
			VOLATILE PARALLEL UNSAFE
			ROWS 1000

		AS $BODY$
		BEGIN

		RETURN QUERY
		SELECT 
			EXTRACT(YEAR FROM {Col(j => j.Day)}) AS ""year"", 
			EXTRACT(MONTH FROM {Col(j => j.Day)}) AS ""month""
		FROM
			{Constants.Schema}.{JourneyTable.TableName}
		WHERE
			{Col(j => j.UserId)} = user_id
		GROUP BY
			""year"", ""month""
		ORDER BY
			""year"" DESC,
			""month"" DESC
		LIMIT
			months
		;

		END;
		$BODY$;
	");

	/// <summary>
	/// 7: Down
	/// </summary>
	protected override void Down() => Execute($@"
		DROP FUNCTION IF EXISTS {Constants.Schema}.{Constants.Functions.GetExpensesReportRecentMonths}(bigint, integer)
		;
	");
}
