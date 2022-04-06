// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Messages;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SavePlace.Messages;

/// <summary>Place does not belong to a user</summary>
/// <param name="UserId"></param>
/// <param name="PlaceId"></param>
public sealed record class PlaceDoesNotBelongToUserMsg(AuthUserId UserId, PlaceId PlaceId) : Msg;
