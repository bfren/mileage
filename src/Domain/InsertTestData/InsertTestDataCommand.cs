// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;

namespace Mileage.Domain.InsertTestData;

/// <inheritdoc cref="InsertTestDataHandler"/>
public sealed record class InsertTestDataCommand : Command;
