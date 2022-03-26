// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common;

namespace Mileage.Queries.SaveSettings.Internals;

/// <summary>
/// Create a settings entity
/// </summary>
/// <param name="UserId"></param>
/// <param name="Settings"></param>
public sealed record class CreateSettingsCommand(
	AuthUserId UserId,
	Settings Settings
) : ICommand;
