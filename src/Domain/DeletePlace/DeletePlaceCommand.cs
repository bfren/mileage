// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.Ids;

namespace Mileage.Domain.DeletePlace;

/// <inheritdoc cref="DeletePlaceHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="Id">Place ID</param>
public sealed record class DeletePlaceCommand(
	AuthUserId UserId,
	PlaceId Id
) : Command;
