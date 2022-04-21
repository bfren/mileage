// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.GetPlace;

/// <summary>
/// From Place model
/// </summary>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="Description"></param>
/// <param name="Postcode"></param>
public sealed record class GetPlaceModel(
	PlaceId Id,
	long Version,
	string Description,
	string? Postcode
)
{
	/// <summary>
	/// Create blank for model binding
	/// </summary>
	public GetPlaceModel() : this(new(), 0L, string.Empty, null) { }
}
