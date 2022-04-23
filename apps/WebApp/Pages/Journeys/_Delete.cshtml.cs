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

public sealed class DeleteModel : DeleteModalModel
{
	public GetJourneyModel Journey { get; set; } = new();

	public DeleteModel() : base("Journey") { }
}

public sealed partial class IndexModel
{
	public Task<PartialViewResult> OnGetDeleteAsync(JourneyId journeyId)
	{
		// Create query
		var query = from u in User.GetUserId()
					from c in Dispatcher.DispatchAsync(new GetJourneyQuery(u, journeyId))
					select c;

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync(
				some: x => Partial("_Delete", new DeleteModel { Journey = x }),
				none: r => Partial("Modals/ErrorModal", r)
			);
	}

	public Task<IActionResult> OnPostDeleteAsync(DeleteJourneyCommand journey)
	{
		var query = from u in User.GetUserId()
					from r in Dispatcher.DispatchAsync(journey with { UserId = u })
					select r;

		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync<bool, IActionResult>(
				some: async x => x switch
				{
					true =>
						await OnGetIncompleteAsync(),

					false =>
						Result.Error("Unable to delete Journey.")
				},
				none: r => Result.Error(r)
			);
	}
}
