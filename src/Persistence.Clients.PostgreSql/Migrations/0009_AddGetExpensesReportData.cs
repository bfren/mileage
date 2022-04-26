// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.Reports;
using Mileage.Persistence.Tables;
using SimpleMigrations;

namespace Mileage.Persistence.Clients.PostgreSql.Migrations;

/// <summary>
/// Migration 0009: Add function to get expenses report data
/// </summary>
[Migration(9, "Add function to get expenses report data")]
public sealed class AddGetExpensesReportDataWithCarNumberPlate : Migration
{
	private string J(Func<JourneyTable, string> selector) =>
		$"{A.Tbl.Journey}.{selector(new())}";

	private string C(Func<CarTable, string> selector) =>
		$"{A.Tbl.Car}.{selector(new())}";

	private string R(Func<RateTable, string> selector) =>
		$"{A.Tbl.Rate}.{selector(new())}";

	private string F(Func<PlaceTable, string> selector) =>
		$"{A.Tbl.From}.{selector(new())}";

	private string T(Func<PlaceTable, string> selector) =>
		$"{A.Tbl.To}.{selector(new())}";

	/// <summary>
	/// 8: Up
	/// </summary>
	protected override void Up() => Execute($@"
		CREATE OR REPLACE FUNCTION {Constants.Schema}.{Constants.Functions.GetExpensesReportData}(
			{A.Var.UserId} bigint,
			{A.Var.From} timestamp without time zone,
			{A.Var.To} timestamp without time zone)
			RETURNS TABLE(
				""{A.Col.Id}"" integer,
				""{A.Col.Date}"" date,
				""{A.Col.Start}"" integer,
				""{A.Col.End}"" integer,
				""{A.Col.Distance}"" integer,
				""{A.Col.Car}"" text,
				""{A.Col.Rate}"" numeric(4,2),
				""{A.Col.From}"" jsonb,
				""{A.Col.To}"" jsonb
			) 
			LANGUAGE 'plpgsql'
			COST 100
			VOLATILE PARALLEL UNSAFE
			ROWS 1000

		AS $BODY$
		BEGIN

		RETURN QUERY
		SELECT 
			{J(x => x.Id)} AS ""{A.Col.Id}"",
			{J(x => x.Day)} AS ""{A.Col.Date}"",
			{J(x => x.StartMiles)} AS  ""{A.Col.Start}"",
			{J(x => x.EndMiles)} AS ""{A.Col.End}"",
			{J(x => x.EndMiles)} - {J(x => x.StartMiles)} AS ""{A.Col.Distance}"",
			{C(x => x.NumberPlate)} AS ""{A.Col.Car}"",
			{R(x => x.AmountPerMileGBP)} AS  ""{A.Col.Rate}"",
			jsonb_build_object(
				'{A.Col.Description.ToLowerInvariant()}', {F(x => x.Description)},
				'{A.Col.Postcode.ToLowerInvariant()}', {F(x => x.Postcode)}
			) AS ""{A.Col.From}"",
			(
				SELECT jsonb_agg(
					jsonb_build_object(
						'{A.Col.Description.ToLowerInvariant()}', {T(x => x.Description)},
						'{A.Col.Postcode.ToLowerInvariant()}', {T(x => x.Postcode)}
					)
				)
				FROM {Constants.Schema}.{PlaceTable.TableName} {A.Tbl.To}
				WHERE {J(x => x.ToPlaceIds)} ? {T(x => x.Id)}::text
			) AS ""{A.Col.To}""
		FROM
			{Constants.Schema}.{JourneyTable.TableName} {A.Tbl.Journey}
			LEFT JOIN {Constants.Schema}.{CarTable.TableName} {A.Tbl.Car} ON {J(x => x.CarId)} = {C(x => x.Id)}
			LEFT JOIN {Constants.Schema}.{PlaceTable.TableName} {A.Tbl.From} ON {J(x => x.FromPlaceId)} = {F(x => x.Id)}
			LEFT JOIN {Constants.Schema}.{RateTable.TableName} {A.Tbl.Rate} ON {J(x => x.RateId)} = {R(x => x.Id)}
		WHERE
			{J(x => x.UserId)} = {A.Var.UserId}
			AND {J(x => x.Day)} BETWEEN {A.Var.From} AND {A.Var.To}
		ORDER BY
			{J(x => x.EndMiles)} IS NULL DESC,
			{J(x => x.Day)} ASC,
			{J(x => x.StartMiles)} ASC
		;

		END;
		$BODY$;
	");

	/// <summary>
	/// 9: Down
	/// </summary>
	protected override void Down() => Execute($@"
		DROP FUNCTION IF EXISTS {Constants.Schema}.{Constants.Functions.GetExpensesReportData}(
			integer, date, integer, integer, text, numeric(4,2), jsonb, jsonb
		)
		;
	");

	internal static class A
	{
		internal static class Var
		{
			internal const string UserId = "user_id";
			internal const string From = "date_from";
			internal const string To = "date_to";
		}

		internal static class Tbl
		{
			internal const string Journey = "j";
			internal const string Car = "c";
			internal const string Rate = "r";
			internal const string From = "f";
			internal const string To = "t";
		}

		internal static class Col
		{
			internal const string Id = nameof(ExpensesReportJourney.Id);
			internal const string Date = nameof(ExpensesReportJourney.Day);
			internal const string Start = nameof(ExpensesReportJourney.Start);
			internal const string End = nameof(ExpensesReportJourney.End);
			internal const string Distance = nameof(ExpensesReportJourney.Distance);
			internal const string Car = nameof(ExpensesReportJourney.Car);
			internal const string Rate = nameof(ExpensesReportJourney.Rate);
			internal const string From = nameof(ExpensesReportJourney.From);
			internal const string To = nameof(ExpensesReportJourney.To);

			internal const string Description = nameof(ExpensesReportPlace.Description);
			internal const string Postcode = nameof(ExpensesReportPlace.Postcode);
		}
	}
}
