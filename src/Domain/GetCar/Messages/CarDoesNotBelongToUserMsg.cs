// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Messages;

namespace Mileage.Domain.GetCar.Messages;

/// <summary>Requested car does not belong to the user</summary>
public sealed record class CarDoesNotBelongToUserMsg : Msg;
