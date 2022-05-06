// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mileage.Domain;
using Mileage.Domain.GetJourney;
using Mileage.Persistence.Common.StrongIds;
using Mileage.WebApp.Pages.Modals;
using StrongId;

namespace Mileage.WebApp.Pages.Journeys;

public abstract class EditJourneyModalModel : UpdateModalModel
{
	public GetJourneyModel Journey { get; set; } = new();

	protected EditJourneyModalModel(string title) : base(title) { }

	protected EditJourneyModalModel(string title, string size) : base(title, size) { }
}

public sealed class AddNewItemToJourneyModel
{
	public JourneyId Id { get; set; } = new();

	public long Version { get; set; }

	public string Value { get; set; } = string.Empty;
}

[Authorize]
[ValidateAntiForgeryToken]
public sealed partial class IndexModel : PageModel
{
	public IDispatcher Dispatcher { get; }

	public ILog<IndexModel> Log { get; }

	public IndexModel(IDispatcher dispatcher, ILog<IndexModel> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	#region Update Fields

	private Task<PartialViewResult> GetFieldAsync<TModel>(
		string partial,
		JourneyId journeyId,
		Func<GetJourneyModel, TModel> getModel
	) where TModel : EditJourneyModalModel =>
		GetFieldAsync(partial, journeyId, _ => F.True.AsTask, (j, _) => getModel(j));

	private Task<PartialViewResult> GetFieldAsync<TValue, TModel>(
		string partial,
		JourneyId journeyId,
		Func<AuthUserId, Task<Maybe<TValue>>> getValue,
		Func<GetJourneyModel, TValue, TModel> getModel
	) where TModel : EditJourneyModalModel
	{
		Log.Vrb("Getting Journey {JourneyId} and {Values}.", journeyId, typeof(TValue));

		// Build the query
		var query = from userId in User.GetUserId()
					from journey in Dispatcher.DispatchAsync(new GetJourneyQuery(userId, journeyId))
					from value in getValue(userId)
					select new { journey, value };

		// Execute query and return partial
		return query
			.SwitchAsync(
				some: x => Partial("_Edit" + partial, getModel(x.journey, x.value)),
				none: r => Partial("Modals/ErrorModal", r)
			);
	}

	private Task<IActionResult> PostFieldAsync<TCommand, TValue>(
		string component,
		string? description,
		TCommand command,
		Func<TCommand, TValue> getValue,
		string? handler = null
	)
		where TCommand : Command, IWithId<JourneyId>, IWithUserId
	{
		// Get values
		var journeyId = command.Id;
		var label = description ?? component;
		var value = getValue(command);
		var updateUrl = Url.Page(pageName: "Index", pageHandler: "Edit" + (handler ?? component), values: new { journeyId = journeyId.Value });

		// Log operation
		Log.Vrb("Saving {Label} for Journey {JourneyId}.", label, journeyId);

		// Build the query
		var query = from userId in User.GetUserId()
					from result in Dispatcher.DispatchAsync(command with { UserId = userId })
					select result;

		// Execute query and return result
		return query
			.AuditAsync(none: Log.Msg)
			.SwitchAsync<bool, IActionResult>(
				some: x => x switch
				{
					true =>
						ViewComponent(component, new { label, updateUrl, value, journeyId }),

					false =>
						Result.Error($"Unable to save {label}.")
				},
				none: r => Result.Error(r)
			);
	}

	private Task<IActionResult> PostCreateItemAsync<TId>(
		Func<AuthUserId, Task<Maybe<TId>>> saveItem,
		Func<AuthUserId, TId, Task<IActionResult>> addItemToJourney
	)
	{
		var query = from userId in User.GetUserId()
					from itemId in saveItem(userId)
					select new { userId, itemId };

		return query.SwitchAsync(
			some: x => addItemToJourney(x.userId, x.itemId),
			none: r => Result.Error(r)
		);
	}

	#endregion Update Fields
}
