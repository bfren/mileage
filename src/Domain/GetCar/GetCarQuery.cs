// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.GetCar;

public sealed record class GetCarQuery(
	AuthUserId UserId,
	CarId CarId
) : IQuery<GetCarModel>;
