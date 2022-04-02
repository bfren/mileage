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
internal sealed record class CarToDelete(CarId Id, long Version) : IWithVersion<CarId>
{
	public CarToDelete() : this(new(), 0L) { }
}
