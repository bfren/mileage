// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SavePlace.Internals;

/// <inheritdoc cref="UpdatePlaceHandler"/>
/// <param name="PlaceId">Car ID</param>
/// <param name="Version">Entity Version</param>
/// <param name="Description">Description</param>
/// <param name="Postcode">Postcode</param>
internal sealed record class UpdatePlaceCommand(
	PlaceId PlaceId,
	long Version,
	string Description,
	string? Postcode
) : ICommand;
