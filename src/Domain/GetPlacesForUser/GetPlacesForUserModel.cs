// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.GetPlacesForUser;

public sealed record class GetPlacesForUserModel(PlaceId Id, string Description);
