// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Collections.Generic;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;

namespace Mileage.Domain.GetIncompleteJourneys;

/// <inheritdoc cref="GetIncompleteJourneysHandler"/>
/// <param name="UserId"></param>
public sealed record class GetIncompleteJourneysQuery(
	AuthUserId UserId
) : IQuery<IEnumerable<IncompleteJourneyModel>>;
