// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common;

namespace Mileage.Domain.LoadSettings;

/// <inheritdoc cref="LoadSettingsHandler"/>
/// <param name="Id"></param>
public sealed record class LoadSettingsQuery(
	AuthUserId Id
) : IQuery<Settings>;
