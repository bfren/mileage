// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SavePlace.Internals;

/// <inheritdoc cref="CreatePlaceHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="Description">Place Description</param>
/// <param name="Postcode">Place Postcode</param>
internal sealed record class CreatePlaceQuery(
	AuthUserId UserId,
	string Description,
	string? Postcode
) : IQuery<PlaceId>;
