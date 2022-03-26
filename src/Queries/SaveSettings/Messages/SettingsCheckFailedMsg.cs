// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Messages;

namespace Mileage.Domain.SaveSettings.Messages;

/// <summary>Pre-save settings checks have failed</summary>
public sealed record class SettingsCheckFailedMsg : Msg;
