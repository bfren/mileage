// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common;

namespace Mileage.Domain.SaveSettings;

/// <inheritdoc cref="SaveSettingsHandler"/>
/// <param name="UserId"></param>
/// <param name="Settings"></param>
public sealed record class SaveSettingsCommand(
	AuthUserId UserId,
	Settings Settings
) : Command;
