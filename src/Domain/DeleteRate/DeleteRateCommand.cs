// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.DeleteRate;

/// <inheritdoc cref="DeleteRateHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="RateId">Rate ID</param>
public sealed record class DeleteRateCommand(
	AuthUserId UserId,
	RateId RateId
) : ICommand;
