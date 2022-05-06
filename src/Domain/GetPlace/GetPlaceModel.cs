// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.GetPlace;

/// <summary>
/// From Place model
/// </summary>
/// <param name="UserId"></param>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="Description"></param>
/// <param name="Postcode"></param>
/// <param name="IsDisabled"></param>
public sealed record class GetPlaceModel(
	AuthUserId UserId,
	PlaceId Id,
	long Version,
	string Description,
	string? Postcode,
	bool IsDisabled
) : IWithUserId, IWithId<PlaceId>
{
	/// <summary>
	/// Create blank for model binding
	/// </summary>
	public GetPlaceModel() : this(new(), new(), 0L, string.Empty, null, false) { }
}
