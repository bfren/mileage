// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;

namespace Mileage.Domain.GetGetAnnualMileageReportYears;

/// <summary>
/// Tax Year model
/// </summary>
/// <param name="StartYear"></param>
public sealed record class TaxYearModel(
	int StartYear
)
{
	/// <summary>
	/// End year for a tax year
	/// </summary>
	public int EndYear =>
		StartYear + 1;

	/// <summary>
	/// Start Date for a tax year
	/// </summary>
	public DateTime StartDate =>
		new(StartYear, 4, 6);

	/// <summary>
	/// End date for a tax year
	/// </summary>
	public DateTime EndDate =>
		new(EndYear, 4, 5);

	/// <summary>
	/// Create blank for model binding
	/// </summary>
	public TaxYearModel() : this(0) { }
}
