// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Messages;

namespace Mileage.Domain.CheckRateCanBeDeleted.Messages;

/// <summary>
/// Requested rate is the user's default rate so cannot be deleted or disabled
/// </summary>
public sealed record class RateIsDefaultRateMsg : Msg;
