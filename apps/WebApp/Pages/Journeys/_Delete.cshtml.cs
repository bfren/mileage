// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain.DeleteJourney;
using Mileage.Domain.GetJourney;
using Mileage.Persistence.Common.StrongIds;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Journeys;

public enum JourneyList
{
	Incomplete = 1 << 0,
	Recent = 1 << 1
}

public sealed class DeleteModel : DeleteModalModel
{
	public GetJourneyModel Journey { get; set; } = new();

	public JourneyList Type { get; set; }

	public DeleteModel() : base("Journey") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetDeleteIncompleteAsync(JourneyId journeyId) =>
		GetDeleteAsync(journeyId, JourneyList.Incomplete);

	public Task<PartialViewResult> OnGetDeleteRecentAsync(JourneyId journeyId) =>
		GetDeleteAsync(journeyId, JourneyList.Recent);

	private Task<PartialViewResult> GetDeleteAsync(JourneyId journeyId, JourneyList type)
	{
		// Create query
		var query = from u in User.GetUserId()
					from c in Dispatcher.DispatchAsync(new GetJourneyQuery(u, journeyId))
					select c;

		// Execute and return partial
		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("_Delete", new DeleteModel { Journey = x, Type = type }),
				none: r => Partial("Modals/ErrorModal", r)
			);
	}

	public Task<IActionResult> OnPostDeleteAsync(DeleteJourneyCommand journey, JourneyList type)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.DispatchAsync(journey with { UserId = u })
					select r;

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync<bool, IActionResult>(
				some: async x => x switch
				{
					true when type == JourneyList.Incomplete =>
						await OnGetIncompleteAsync(),

					true when type == JourneyList.Recent =>
						await OnGetRecentAsync(),

					_ =>
						Result.Error("Unable to delete Journey.")
				},
				none: r => Result.Error(r)
			);
	}
}
