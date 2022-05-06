// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.DeleteCar;

/// <inheritdoc cref="DeleteCarHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="Id">Car ID</param>
public sealed record class DeleteCarCommand(
	AuthUserId UserId,
	CarId Id
) : Command;
