// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Collections.Generic;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;

namespace Mileage.Domain.GetPlaces;

/// <inheritdoc cref="GetPlacesHandler"/>
/// <param name="UserId"></param>
public sealed record class GetPlacesQuery(
	AuthUserId UserId
) : IQuery<IEnumerable<GetPlacesModel>>;
