// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.GetIncompleteJourneys;
using Mileage.Domain.GetRecentJourneys;

namespace Mileage.WebApp.Pages.Journeys;

public sealed record class ListsModel
{
	public IncompleteModel IncompleteJourneys { get; set; } = new();

	public RecentModel RecentJourneys { get; set; } = new();
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetListsAsync() =>
		CreateListsModel(
			User.GetUserId(), Dispatcher, Log
		)
		.SwitchAsync(
			some: x => Partial("_Lists", x),
			none: r => Partial("_Error", r)
		);

	internal static Task<Maybe<ListsModel>> CreateListsModel(Maybe<AuthUserId> user, IDispatcher dispatcher, ILog log)
	{
		var query = from u in user
					from incomplete in dispatcher.DispatchAsync(new GetIncompleteJourneysQuery(u))
					from recent in dispatcher.DispatchAsync(new GetRecentJourneysQuery(u))
					select new { incomplete, recent };

		return query
			.AuditAsync(none: log.Msg)
			.MapAsync(
				x => new ListsModel
				{
					IncompleteJourneys = new() { Journeys = x.incomplete.ToList() },
					RecentJourneys = new() { Journeys = x.recent.ToList() }
				},
				F.DefaultHandler
			);
	}
}
