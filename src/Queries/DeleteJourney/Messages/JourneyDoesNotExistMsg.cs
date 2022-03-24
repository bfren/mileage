// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Messages;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Queries.DeleteJourney.Messages;

/// <summary>
/// The Journey does not exist, or does not belong to the specified User
/// </summary>
/// <param name="JourneyId">Journey ID</param>
/// <param name="UserId">User ID</param>
public sealed record class JourneyDoesNotExistMsg(JourneyId JourneyId, AuthUserId UserId) : Msg;
