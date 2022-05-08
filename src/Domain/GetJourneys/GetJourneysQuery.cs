// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using System.Collections.Generic;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;

namespace Mileage.Domain.GetJourneys;

/// <inheritdoc cref="GetJourneysHandler"/>
/// <param name="UserId"></param>
/// <param name="Start"></param>
/// <param name="End"></param>
public sealed record class GetJourneysQuery(
	AuthUserId UserId,
	DateTime Start,
	DateTime End
) : Query<IEnumerable<JourneyModel>>;
