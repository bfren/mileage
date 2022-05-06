// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SavePlace;

/// <inheritdoc cref="SavePlaceHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="Id">Place ID</param>
/// <param name="Version">Entity Verion</param>
/// <param name="Description">Description</param>
/// <param name="Postcode">Postcode</param>
/// <param name="IsDisabled"></param>
public sealed record class SavePlaceQuery(
	AuthUserId UserId,
	PlaceId? Id,
	long Version,
	string Description,
	string? Postcode,
	bool IsDisabled
) : Query<PlaceId>
{
	/// <summary>
	/// Save with minimum required values (for new places)
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="description"></param>
	public SavePlaceQuery(AuthUserId userId, string description) : this(
		UserId: userId,
		Id: null,
		Version: 0L,
		Description: description,
		Postcode: null,
		IsDisabled: false
	)
	{ }

	/// <summary>
	/// Create blank for model binding
	/// </summary>
	public SavePlaceQuery() : this(new(), string.Empty) { }
}
