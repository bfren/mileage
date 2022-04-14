// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Messages;

namespace Mileage.Domain.GetFromPlace.Messages;

/// <summary>Requested PlaceId is not set</summary>
public sealed record class PlaceIdIsNullMsg : Msg;
