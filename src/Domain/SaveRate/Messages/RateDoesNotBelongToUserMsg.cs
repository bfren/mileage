// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Messages;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveRate.Messages;

/// <summary>Car does not belong to a user</summary>
/// <param name="UserId"></param>
/// <param name="RateId"></param>
public sealed record class RateDoesNotBelongToUserMsg(AuthUserId UserId, RateId RateId) : Msg;
