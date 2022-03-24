// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Id;

namespace Mileage.Persistence.Common.StrongIds;

/// <summary>
/// Rate ID
/// </summary>
/// <param name="Value">ID Value</param>
public readonly record struct RateId(long Value) : IStrongId;
