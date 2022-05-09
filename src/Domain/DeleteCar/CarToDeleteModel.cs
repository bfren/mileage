// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.DeleteCar;

/// <summary>
/// Used to return a car that is ready to be deleted
/// </summary>
/// <param name="Id">Car ID</param>
/// <param name="Version">Concurrency version</param>
/// <param name="IsDisabled"></param>
internal sealed record class CarToDeleteModel(CarId Id, long Version, bool IsDisabled) : IWithVersion<CarId>
{
	public CarToDeleteModel() : this(new(), 0L, false) { }
}
