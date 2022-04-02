// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Messages;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.DeletePlace.Messages;

/// <summary>
/// The place does not exist, or does not belong to the specified user
/// </summary>
/// <param name="UserId">User ID</param>
/// <param name="PlaceId">Place ID</param>
public sealed record class PlaceDoesNotExistMsg(AuthUserId UserId, PlaceId PlaceId) : Msg;
