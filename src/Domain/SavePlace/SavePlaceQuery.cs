// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SavePlace;

/// <inheritdoc cref="SavePlaceHandler"/>
/// <param name="UserId">User ID</param>
/// <param name="PlaceId">Place ID</param>
/// <param name="Version">Entity Verion</param>
/// <param name="Description">Description</param>
/// <param name="Postcode">Postcode</param>
public sealed record class SavePlaceQuery(
	AuthUserId UserId,
	PlaceId? PlaceId,
	long Version,
	string Description,
	string? Postcode
) : IQuery<PlaceId>
{
	/// <summary>
	/// Save with minimum required values (for new places)
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="description"></param>
	public SavePlaceQuery(AuthUserId userId, string description) : this(
		UserId: userId,
		PlaceId: null,
		Version: 0L,
		Description: description,
		Postcode: null
	)
	{ }
}
