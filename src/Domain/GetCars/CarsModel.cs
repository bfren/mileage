// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.GetCars;

/// <summary>
/// Car model
/// </summary>
/// <param name="Id"></param>
/// <param name="Description"></param>
/// <param name="NumberPlate"></param>
/// <param name="IsDisabled"></param>
public sealed record class CarsModel(
	CarId Id,
	string Description,
	string? NumberPlate,
	bool IsDisabled
) : IWithId<CarId>;
