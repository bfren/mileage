// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using System.Collections.Generic;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.Reports;

namespace Mileage.Domain.GetAnnualMileageReportData;

/// <inheritdoc cref="GetAnnualMileageReportDataHandler"/>
/// <param name="UserId"></param>
/// <param name="From"></param>
/// <param name="To"></param>
public sealed record class GetAnnualMileageReportDataQuery(
	AuthUserId UserId,
	DateTime From,
	DateTime To
) : Query<IEnumerable<AnnualMileageReportJourney>>;
