// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveRate.Internals;

/// <inheritdoc cref="UpdateRateHandler"/>
/// <param name="RateId">Rate ID</param>
/// <param name="Version">Entity Version</param>
/// <param name="AmountPerMileGBP">Amount per Mile (in GBP)</param>
internal sealed record class UpdateRateCommand(
	RateId RateId,
	long Version,
	float AmountPerMileGBP
) : ICommand;
