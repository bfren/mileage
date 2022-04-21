// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.GetPlace;

/// <inheritdoc cref="GetPlaceHandler"/>
/// <param name="UserId"></param>
/// <param name="PlaceId"></param>
public sealed record class GetPlaceQuery(
	AuthUserId UserId,
	PlaceId PlaceId
) : IQuery<GetPlaceModel>;
