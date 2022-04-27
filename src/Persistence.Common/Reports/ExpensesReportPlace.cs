// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

namespace Mileage.Persistence.Common.Reports;

/// <summary>
/// Expenses Report: Place model
/// </summary>
public sealed record class ExpensesReportPlace
{
	/// <summary>
	/// Place Description
	/// </summary>
	public string Description { get; init; } = string.Empty;

	/// <summary>
	/// Place Postcode
	/// </summary>
	public string? Postcode { get; init; }
}
