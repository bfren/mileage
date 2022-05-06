// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;

namespace Mileage.Domain.MigrateToLatest;

/// <inheritdoc cref="MigrateToLatestHandler"/>
public sealed record class MigrateToLatestCommand : Command;
