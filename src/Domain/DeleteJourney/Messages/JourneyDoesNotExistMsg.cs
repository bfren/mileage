// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Messages;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.DeleteJourney.Messages;

/// <summary>
/// The Journey does not exist, or does not belong to the specified User
/// </summary>
/// <param name="UserId">User ID</param>
/// <param name="JourneyId">Journey ID</param>
public sealed record class JourneyDoesNotExistMsg(AuthUserId UserId, JourneyId JourneyId) : Msg;
