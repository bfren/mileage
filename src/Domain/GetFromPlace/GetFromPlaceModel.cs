// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.GetFromPlace;

/// <summary>
/// From Place model
/// </summary>
/// <param name="Id"></param>
/// <param name="Description"></param>
public sealed record class GetFromPlaceModel(PlaceId Id, string Description);
