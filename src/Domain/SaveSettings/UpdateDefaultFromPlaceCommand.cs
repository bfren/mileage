// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.Ids;
using Wrap.Ids;

namespace Mileage.Domain.SaveSettings;

/// <inheritdoc cref="UpdateDefaultFromPlaceHandler"/>
/// <param name="UserId"></param>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="DefaultFromPlaceId"></param>
public sealed record class UpdateDefaultFromPlaceCommand(
	AuthUserId UserId,
	SettingsId Id,
	long Version,
	PlaceId? DefaultFromPlaceId
) : Command, IWithId<SettingsId, long>, IWithUserId;
