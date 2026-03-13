// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Jeebs.Mvc;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mileage.Domain;
using Mileage.Domain.LoadSettings;
using Mileage.WebApp.Pages.Modals;

namespace Mileage.WebApp.Pages.Settings.General;

public abstract class EditSettingsModel : UpdateModalModel
{
	public Persistence.Common.Settings Settings { get; set; } = new();

	protected EditSettingsModel(string title) : base(title) { }
}

[Authorize]
[ValidateAntiForgeryToken]
public sealed partial class IndexModel : PageModel
{
	private IDispatcher Dispatcher { get; }

	private ILog<IndexModel> Log { get; }

	public Persistence.Common.Settings Settings { get; set; } = new();

	public IndexModel(IDispatcher dispatcher, ILog<IndexModel> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	public async Task<IActionResult> OnGetAsync()
	{
		var query = from u in User.GetUserId()
					from s in Dispatcher.SendAsync(new LoadSettingsQuery(u))
					select s;

		foreach (var settings in await query.AuditAsync(fFail: Log.Failure).Unsafe())
		{
			Settings = settings;
		}

		return Page();
	}

	private Task<PartialViewResult> GetFieldAsync<TValue, TModel>(
		string partial,
		Func<AuthUserId, Task<Result<TValue>>> getValue,
		Func<Persistence.Common.Settings, TValue, TModel> getModel
	) where TModel : EditSettingsModel
	{
		// Build query
		var query = from u in User.GetUserId()
					from settings in Dispatcher.SendAsync(new LoadSettingsQuery(u))
					from value in getValue(u)
					select new { settings, value };

		// Execute query and return partial
		return query
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync(
				fOk: x => Partial("_Edit" + partial, getModel(x.settings, x.value)),
				fFail: r => Partial("Modals/ErrorModal", r)
			);
	}

	private Task<IActionResult> PostFieldAsync<TCommand, TValue>(
		string component,
		string label,
		TCommand command,
		Func<TCommand, TValue> getValue
	)
		where TCommand : Command, IWithUserId
	{
		// Get values
		var updateUrl = Url.Page("Index", "Edit" + component);
		var value = getValue(command);

		// Log operation
		Log.Vrb("Saving {Setting} for {User}.", component, User.GetUserId());

		// Build query
		var query = from userId in User.GetUserId()
					from result in Dispatcher.SendAsync(command with { UserId = userId })
					select result;

		return query
			.AuditAsync(fFail: Log.Failure)
			.MatchAsync<bool, IActionResult>(
				fOk: x => x switch
				{
					true =>
						ViewComponent(component, new { label, updateUrl, value }),

					false =>
						Op.Error($"Unable to save {label}.")
				},
				fFail: Op.Error
			);
	}
}
