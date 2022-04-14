// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.GetRates;

/// <summary>
/// Rate model
/// </summary>
/// <param name="Id"></param>
/// <param name="AmountPerMileGBP"></param>
public sealed record class GetRatesModel(RateId Id, float AmountPerMileGBP) : IWithId<RateId>;
