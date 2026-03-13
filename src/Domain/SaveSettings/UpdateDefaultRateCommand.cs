// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.Ids;
using Wrap.Ids;

namespace Mileage.Domain.SaveSettings;

/// <inheritdoc cref="UpdateDefaultRateHandler"/>
/// <param name="UserId"></param>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="DefaultRateId"></param>
public sealed record class UpdateDefaultRateCommand(
	AuthUserId UserId,
	SettingsId Id,
	long Version,
	RateId? DefaultRateId
) : Command, IWithId<SettingsId, long>, IWithUserId;
