// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Messages;

namespace Mileage.Domain.CheckPlacesBelongToUser.Messages;

/// <summary>List of PlaceIds is null or empty</summary>
public sealed record class PlaceIdsIsNullMsg : Msg;
