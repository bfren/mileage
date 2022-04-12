// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.GetPlaces;

public sealed record class GetPlacesModel(PlaceId Id, string Description) : IWithId<PlaceId>;
