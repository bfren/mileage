// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using System.Collections.Generic;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Persistence.Common.Reports;

public sealed record class ExpensesReportJourney
{
	public JourneyId Id { get; init; } = new();

	public DateTime Day { get; init; }

	public int Start { get; init; }

	public int End { get; init; }

	public int? Distance { get; init; }

	public string Car { get; init; } = string.Empty;

	public float Rate { get; init; }

	public ExpensesReportPlace From { get; init; } = new();

	public List<ExpensesReportPlace> To { get; init; } = new();
}
