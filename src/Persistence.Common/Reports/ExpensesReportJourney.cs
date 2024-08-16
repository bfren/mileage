// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using System.Collections.Generic;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Persistence.Common.Reports;

/// <summary>
/// Expenses Report: Journey model
/// </summary>
public sealed record class ExpensesReportJourney
{
	/// <summary>
	/// Journey ID
	/// </summary>
	public JourneyId Id { get; init; } = new();

	/// <summary>
	/// Journey Day
	/// </summary>
	public DateTime Day { get; init; }

	/// <summary>
	/// Journey Start Miles
	/// </summary>
	public int Start { get; init; }

	/// <summary>
	/// Journey End Miles
	/// </summary>
	public int End { get; init; }

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

	/// <summary>
	/// Journey Starting Place
	/// </summary>
	public ExpensesReportPlace From { get; init; } = new();

	/// <summary>
	/// Journey Destinations
	/// </summary>
	public List<ExpensesReportPlace> To { get; init; } = [];
}
