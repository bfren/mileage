// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.Ids;

namespace Mileage.Domain.CheckCarCanBeDeleted;

/// <inheritdoc cref="CheckCarCanBeDeletedHandler"/>
/// <param name="UserId"></param>
/// <param name="Id"></param>
public sealed record class CheckCarCanBeDeletedQuery(
	AuthUserId UserId,
	CarId Id
) : Query<DeleteOperation>, IWithId<CarId, long>, IWithUserId;
