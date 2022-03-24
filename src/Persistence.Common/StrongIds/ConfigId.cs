// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Id;

namespace Mileage.Persistence.Common.StrongIds;

/// <summary>
/// Config ID
/// </summary>
/// <param name="Value">ID value</param>
public readonly record struct ConfigId(long Value) : IStrongId;
