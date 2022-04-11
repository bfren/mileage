// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.GetCarsForUser;

public sealed record class GetCarsForUserModel(CarId Id, string Description);
