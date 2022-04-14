// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Messages;

namespace Mileage.Domain.GetCar.Messages;

/// <summary>Requested CarId is not set</summary>
public sealed record class CarIdIsNullMsg : Msg;
