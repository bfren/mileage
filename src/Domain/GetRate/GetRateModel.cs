// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.GetRate;

/// <summary>
/// Car model
/// </summary>
/// <param name="Id"></param>
/// <param name="AmountPerMileInGBP"></param>
public sealed record class GetRateModel(RateId Id, float AmountPerMileInGBP);
