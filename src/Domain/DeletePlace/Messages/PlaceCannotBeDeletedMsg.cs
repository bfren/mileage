// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Messages;

namespace Mileage.Domain.DeletePlace.Messages;

/// <summary>
/// Invalid delete operation
/// </summary>
public sealed record class PlaceCannotBeDeletedMsg : Msg;
