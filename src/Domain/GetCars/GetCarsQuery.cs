// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Collections.Generic;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;

namespace Mileage.Domain.GetCars;

public sealed record class GetCarsQuery(
	AuthUserId UserId
) : IQuery<IEnumerable<GetCarsModel>>;
