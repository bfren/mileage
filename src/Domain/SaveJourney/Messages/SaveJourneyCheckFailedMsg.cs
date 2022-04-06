// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Messages;

namespace Mileage.Domain.SaveJourney.Messages;

/// <summary>Pre-save Journey checks have failed</summary>
public sealed record class SaveJourneyCheckFailedMsg : Msg;
