// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.DeletePlace;

/// <inheritdoc cref="DeletePlaceHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="PlaceId">Place ID</param>
public sealed record class DeletePlaceCommand(
	AuthUserId UserId,
	PlaceId PlaceId
) : ICommand;
