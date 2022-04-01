// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveCar;

/// <inheritdoc cref="SaveCarHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="CarId">Car ID</param>
/// <param name="Version">Entity Verion</param>
/// <param name="Description">Description</param>
public sealed record class SaveCarQuery(
	AuthUserId UserId,
	CarId CarId,
	long Version,
	string Description
) : IQuery<CarId>;
