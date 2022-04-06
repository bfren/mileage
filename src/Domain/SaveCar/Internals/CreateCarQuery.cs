// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveCar.Internals;

/// <inheritdoc cref="CreateCarHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="Description">Description</param>
internal sealed record class CreateCarQuery(
	AuthUserId UserId,
	string Description
) : IQuery<CarId>;
