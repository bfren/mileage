// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Messages;

namespace Mileage.Domain.GetRate.Messages;

/// <summary>Requested place does not belong to the user</summary>
public sealed record class RateDoesNotBelongToUserMsg : Msg;
