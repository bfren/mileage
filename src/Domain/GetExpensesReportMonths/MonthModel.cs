// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

namespace Mileage.Domain.GetExpensesReportMonths;

/// <summary>
/// Holds reference to a year and month for building reports
/// </summary>
/// <param name="Year"></param>
/// <param name="Month"></param>
public sealed record class MonthModel(
	int Year,
	int Month
)
{
	/// <summary>
	/// Create empty for model binding
	/// </summary>
	public MonthModel() : this(0, 0) { }
}
