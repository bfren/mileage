// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.SaveCar.Internals;

/// <inheritdoc cref="UpdateCarHandler"/>
/// <param name="Id">Car ID</param>
/// <param name="Version">Entity Version</param>
/// <param name="Description">Description</param>
/// <param name="NumberPlate">Number Plate</param>
internal sealed record class UpdateCarCommand(
	CarId Id,
	long Version,
	string Description,
	string? NumberPlate
) : ICommand, IWithId<CarId>;
