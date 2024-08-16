// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;

namespace Mileage.Domain.GetGetAnnualMileageReportYears;

/// <summary>
/// Retrieve the Day column from the Journey table
/// </summary>
/// <param name="Day"></param>
internal sealed record class DayModel(DateTime Day);
