// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.GetCars;

public sealed record class GetCarsModel(CarId Id, string Description) : IWithId<CarId>;
