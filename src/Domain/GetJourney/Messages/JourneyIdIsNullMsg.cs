// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Messages;

namespace Mileage.Domain.GetJourney.Messages;

/// <summary>Requested JourneyId is not set</summary>
public sealed record class JourneyIdIsNullMsg : Msg;
