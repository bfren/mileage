// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.Reports;
using Mileage.Persistence.Tables;
using SimpleMigrations;

namespace Mileage.Persistence.Clients.PostgreSql.Migrations;

/// <summary>
/// Migration 0013: Add function to get annual mileage report data
/// </summary>
[Migration(13, "Add function to get annual mileage report data")]
public sealed class AddGetAnnualMileageReportData : Migration
{
	private string J(Func<JourneyTable, string> selector) =>
		$"{A.Tbl.Journey}.{selector(new())}";

	private string C(Func<CarTable, string> selector) =>
		$"{A.Tbl.Car}.{selector(new())}";

	private string R(Func<RateTable, string> selector) =>
		$"{A.Tbl.Rate}.{selector(new())}";

	/// <summary>
	/// 13: Up
	/// </summary>
	protected override void Up() => Execute($@"
		CREATE OR REPLACE FUNCTION {Constants.Schema}.{Constants.Functions.GetAnnualMileageReportData}(
			{A.Var.UserId} bigint,
			{A.Var.From} timestamp without time zone,
			{A.Var.To} timestamp without time zone)
			RETURNS TABLE(
				""{A.Col.Id}"" integer,
				""{A.Col.Distance}"" integer,
				""{A.Col.Car}"" text,
				""{A.Col.Rate}"" numeric(4,2)
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
			{J(x => x.EndMiles)} - {J(x => x.StartMiles)} AS ""{A.Col.Distance}"",
			{C(x => x.NumberPlate)} AS ""{A.Col.Car}"",
			{R(x => x.AmountPerMileGBP)} AS  ""{A.Col.Rate}""
		FROM
			{Constants.Schema}.{JourneyTable.TableName} {A.Tbl.Journey}
			LEFT JOIN {Constants.Schema}.{CarTable.TableName} {A.Tbl.Car} ON {J(x => x.CarId)} = {C(x => x.Id)}
			LEFT JOIN {Constants.Schema}.{RateTable.TableName} {A.Tbl.Rate} ON {J(x => x.RateId)} = {R(x => x.Id)}
		WHERE
			{J(x => x.UserId)} = {A.Var.UserId}
			AND {J(x => x.Day)} BETWEEN {A.Var.From} AND {A.Var.To}
		ORDER BY
			{C(x => x.NumberPlate)} ASC,
			{R(x => x.AmountPerMileGBP)} ASC
		;

		END;
		$BODY$;
	");

	/// <summary>
	/// 13: Down
	/// </summary>
	protected override void Down() => Execute($@"
		DROP FUNCTION IF EXISTS {Constants.Schema}.{Constants.Functions.GetAnnualMileageReportData}(
			integer, integer, text, numeric(4,2)
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
		}

		internal static class Col
		{
			internal const string Id = nameof(AnnualMileageReportJourney.Id);
			internal const string Distance = nameof(AnnualMileageReportJourney.Distance);
			internal const string Car = nameof(AnnualMileageReportJourney.Car);
			internal const string Rate = nameof(AnnualMileageReportJourney.Rate);
		}
	}
}
