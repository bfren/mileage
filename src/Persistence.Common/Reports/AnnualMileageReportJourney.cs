// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Persistence.Common.Reports;

/// <summary>
/// Annual Mileage Report: Rate
/// </summary>
public sealed record class AnnualMileageReportJourney
{
	/// <summary>
	/// Journey ID
	/// </summary>
	public JourneyId Id { get; init; } = new();

	/// <summary>
	/// Journey calculated distance (null if <see cref="End"/> is zero)
	/// </summary>
	public int? Distance { get; init; }

	/// <summary>
	/// Journey Car number plate
	/// </summary>
	public string Car { get; init; } = string.Empty;

	/// <summary>
	/// Journey Rate amount
	/// </summary>
	public float Rate { get; init; }
}
