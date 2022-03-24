// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Id;

namespace Mileage.Persistence.Entities.StrongIds;

/// <summary>
/// Journey ID
/// </summary>
/// <param name="Value">ID value</param>
public readonly record struct JourneyId(long Value) : IStrongId;
