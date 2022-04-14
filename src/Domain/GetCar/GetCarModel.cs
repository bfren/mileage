// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.GetCar;

/// <summary>
/// Car model
/// </summary>
/// <param name="Id"></param>
/// <param name="Description"></param>
public sealed record class GetCarModel(CarId Id, string Description);
