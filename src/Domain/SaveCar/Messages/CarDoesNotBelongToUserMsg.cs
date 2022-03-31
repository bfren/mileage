// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Messages;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveCar.Messages;

/// <summary>Car does not belong to a user</summary>
/// <param name="CarId"></param>
/// <param name="UserId"></param>
public sealed record class CarDoesNotBelongToUserMsg(CarId CarId, AuthUserId UserId) : Msg;
