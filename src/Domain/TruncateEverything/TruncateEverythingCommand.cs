// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;

namespace Mileage.Domain.TruncateEverything;

/// <inheritdoc cref="TruncateEverythingHandler"/>
public sealed record class TruncateEverythingCommand : ICommand;
