// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using System.Collections.Generic;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.Reports;

namespace Mileage.Domain.GetExpensesReportData;

/// <inheritdoc cref="GetExpensesReportDataHandler"/>
/// <param name="UserId"></param>
/// <param name="From"></param>
/// <param name="To"></param>
public sealed record class GetExpensesReportDataQuery(
	AuthUserId UserId,
	DateTime From,
	DateTime To
) : Query<IEnumerable<ExpensesReportJourney>>;
