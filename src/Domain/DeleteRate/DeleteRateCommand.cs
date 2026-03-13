// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.Ids;

namespace Mileage.Domain.DeleteRate;

/// <inheritdoc cref="DeleteRateHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="Id">Rate ID</param>
public sealed record class DeleteRateCommand(
	AuthUserId UserId,
	RateId Id
) : Command;
