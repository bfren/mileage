// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.GetCar;

/// <summary>
/// Car model
/// </summary>
/// <param name="UserId"></param>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="Description"></param>
/// <param name="NumberPlate"></param>
public sealed record class GetCarModel(
	AuthUserId UserId,
	CarId Id,
	long Version,
	string Description,
	string? NumberPlate
) : IWithId<CarId>
{
	/// <summary>
	/// Create empty - for model binding
	/// </summary>
	public GetCarModel() : this(new(), new(), 0L, string.Empty, null) { }
}
