// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetIncompleteJourneys;

namespace Mileage.WebApp.Pages.Journeys;

public sealed record class IncompleteModel
{
	public List<IncompleteJourneyModel> Journeys { get; init; } = [];
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetIncompleteAsync()
	{
		var query = from u in User.GetUserId()
					from j in Dispatcher.DispatchAsync(new GetIncompleteJourneysQuery(u))
					select j;

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("_Incomplete", new IncompleteModel { Journeys = x.ToList() }),
				none: _ => Partial("_Incomplete", new IncompleteModel())
			);
	}
}
