// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.SaveSettings;

/// <inheritdoc cref="UpdateDefaultRateHandler"/>
/// <param name="UserId"></param>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="DefaultRateId"></param>
public sealed record class UpdateDefaultRateCommand(
	AuthUserId UserId,
	SettingsId Id,
	long Version,
	RateId? DefaultRateId
) : WithUserId, ICommand, IWithId<SettingsId>;
