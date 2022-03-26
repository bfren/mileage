// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Mileage.Persistence.Common;
using Mileage.Persistence.Entities;

namespace Mileage.Domain.SaveSettings.Internals;

/// <summary>
/// Update a settings entity
/// </summary>
/// <param name="ExistingSettings"></param>
/// <param name="UpdatedSettings"></param>
public sealed record class UpdateSettingsCommand(
	SettingsEntity ExistingSettings,
	Settings UpdatedSettings
) : ICommand;