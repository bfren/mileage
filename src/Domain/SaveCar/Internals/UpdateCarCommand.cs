// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveCar.Internals;

/// <inheritdoc cref="UpdateCarHandler"/>
/// <param name="CarId">Car ID</param>
/// <param name="Version">Entity Version</param>
/// <param name="Description">Description</param>
internal sealed record class UpdateCarCommand(
	CarId CarId,
	long Version,
	string Description
) : ICommand;
