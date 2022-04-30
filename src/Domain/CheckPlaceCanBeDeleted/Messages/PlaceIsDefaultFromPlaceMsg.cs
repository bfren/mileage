// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Messages;

namespace Mileage.Domain.CheckPlaceCanBeDeleted.Messages;

/// <summary>
/// Requested place is the user's default from place so cannot be deleted or disabled
/// </summary>
public sealed record class PlaceIsDefaultFromPlaceMsg : Msg;
