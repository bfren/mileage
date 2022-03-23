// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Id;

namespace Persistence.Entities.StrongIds;

/// <summary>
/// Car ID
/// </summary>
/// <param name="Value">ID value</param>
public readonly record struct CarId(long Value) : IStrongId;
