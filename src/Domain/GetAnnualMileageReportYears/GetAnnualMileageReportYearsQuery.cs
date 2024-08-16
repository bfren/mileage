// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Linq;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;

namespace Mileage.Domain.GetGetAnnualMileageReportYears;

/// <inheritdoc cref="GetAnnualMileageReportYearsHandler"/>
/// <param name="UserId"></param>
public sealed record class GetAnnualMileageReportYearsQuery(
	AuthUserId UserId
) : Query<IOrderedEnumerable<TaxYearModel>>;
