// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Messages;
using Mileage.Persistence.Common.StrongIds;
using StrongId;

namespace Mileage.Domain.DeleteCar.Messages;

/// <summary>
/// The car does not exist, or does not belong to the specified user
/// </summary>
/// <param name="UserId">User ID</param>
/// <param name="Id">Car ID</param>
public sealed record class CarDoesNotExistMsg(AuthUserId UserId, CarId Id) : Msg, IWithUserId, IWithId<CarId>;
