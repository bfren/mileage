// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

namespace Mileage.Persistence.Common.Reports;

public sealed record class ExpensesReportPlace
{
	public string Description { get; init; } = string.Empty;

	public string? Postcode { get; init; }
}
