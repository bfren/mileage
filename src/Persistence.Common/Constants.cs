// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

namespace Mileage.Persistence.Common;

/// <summary>
/// Common or shared constant values
/// </summary>
public static class Constants
{
	/// <summary>
	/// Database schema name
	/// </summary>
	public static string Schema { get; } = "mileage";

	/// <summary>
	/// Default number of months to generate expense reports for
	/// </summary>
	public static int ExpensesReportMonths { get; } = 6;

	/// <summary>
	/// Function names
	/// </summary>
	public static class Functions
	{
		/// <summary>
		/// Get expenses report recent months
		/// </summary>
		public static string GetExpensesReportRecentMonths { get; } = $"{Schema}.get_expenses_report_recent_months";

		/// <summary>
		/// Get expenses report data
		/// </summary>
		public static string GetExpensesReportData { get; } = $"{Schema}.get_expenses_report_data";
	}
}
