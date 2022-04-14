// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Messages;

namespace Mileage.Domain.GetRate.Messages;

/// <summary>Requested RateId is not set</summary>
public sealed record class RateIdIsNullMsg : Msg;
