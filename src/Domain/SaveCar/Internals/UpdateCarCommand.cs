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
/// <param name="IsDisabled"></param>
internal sealed record class UpdateCarCommand(
	CarId Id,
	long Version,
	string Description,
	string? NumberPlate,
	bool IsDisabled
) : ICommand, IWithId<CarId>
{
	/// <summary>
	/// Create from a <see cref="SaveCarQuery"/>
	/// </summary>
	/// <param name="carId"></param>
	/// <param name="query"></param>
	public UpdateCarCommand(CarId carId, SaveCarQuery query) : this(
		Id: carId,
		Version: query.Version,
		Description: query.Description,
		NumberPlate: query.NumberPlate,
		IsDisabled: query.IsDisabled
	)
	{ }
}
