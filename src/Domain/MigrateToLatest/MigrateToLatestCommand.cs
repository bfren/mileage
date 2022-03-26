// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;

namespace Mileage.Domain.MigrateToLatest;

/// <summary>
/// Migrate to latest database version
/// </summary>
public sealed record class MigrateToLatestCommand : ICommand;
