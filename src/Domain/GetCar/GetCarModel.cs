// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.GetCar;

/// <summary>
/// Car model
/// </summary>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="Description"></param>
/// <param name="NumberPlate"></param>
public sealed record class GetCarModel(CarId Id, long Version, string Description, string? NumberPlate)
{
	public GetCarModel() : this(new(), 0L, string.Empty, null) { }
}
