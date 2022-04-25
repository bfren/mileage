// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.SaveSettings;

/// <inheritdoc cref="UpdateDefaultCarHandler"/>
/// <param name="UserId"></param>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="DefaultCarId"></param>
public sealed record class UpdateDefaultCarCommand(
	AuthUserId UserId,
	SettingsId Id,
	long Version,
	CarId? DefaultCarId
) : WithUserId, ICommand, IWithId<SettingsId>;
