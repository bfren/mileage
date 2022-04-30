// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveCar;

/// <inheritdoc cref="SaveCarHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="Id">Car ID</param>
/// <param name="Version">Entity Verion</param>
/// <param name="Description">Description</param>
/// <param name="NumberPlate">Number Plate</param>
/// <param name="IsDisabled"></param>
public sealed record class SaveCarQuery(
	AuthUserId UserId,
	CarId? Id,
	long Version,
	string Description,
	string? NumberPlate,
	bool IsDisabled
) : IQuery<CarId>
{
	/// <summary>
	/// Create with minimum required values (for new cars)
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="description"></param>
	public SaveCarQuery(AuthUserId userId, string description) : this(
		UserId: userId,
		Id: null,
		Version: 0L,
		Description: description,
		NumberPlate: null,
		IsDisabled: false
	)
	{ }

	/// <summary>
	/// Create empty for model binding
	/// </summary>
	public SaveCarQuery() : this(new(), string.Empty) { }
}
