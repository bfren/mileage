// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Messages;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.DeleteRate.Messages;

/// <summary>
/// The rate does not exist, or does not belong to the specified user
/// </summary>
/// <param name="UserId">User ID</param>
/// <param name="RateId">Rate ID</param>
public sealed record class RateDoesNotExistMsg(AuthUserId UserId, RateId RateId) : Msg;
