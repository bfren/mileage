// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Messages;

namespace Mileage.Domain.SaveSettings.Messages;

/// <summary>Creating default settings for user has failed</summary>
public sealed record class FailedToCreateDefaultSettingsForUserMsg : Msg;
