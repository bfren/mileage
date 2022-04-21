// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.GetPlaces;

/// <summary>
/// Place model
/// </summary>
/// <param name="Id"></param>
/// <param name="Description"></param>
/// <param name="Postcode"></param>
public sealed record class GetPlacesModel(
	PlaceId Id,
	string Description,
	string? Postcode
);
