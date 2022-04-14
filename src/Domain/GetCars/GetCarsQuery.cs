// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Collections.Generic;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;

namespace Mileage.Domain.GetCars;

/// <inheritdoc cref="GetCarsHandler"/>
/// <param name="UserId"></param>
public sealed record class GetCarsQuery(
	AuthUserId UserId
) : IQuery<IEnumerable<GetCarsModel>>;
