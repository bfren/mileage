// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Domain.SaveSettings;
using Mileage.Domain.SaveSettings.Messages;
using Mileage.Persistence.Common;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.LoadSettings;

/// <summary>
/// Load a user's settings
/// </summary>
internal sealed class LoadSettingsHandler : QueryHandler<LoadSettingsQuery, Settings>
{
	private IDispatcher Dispatcher { get; init; }

	private ILog<LoadSettingsHandler> Log { get; init; }

	private ISettingsRepository Settings { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="settings"></param>
	/// <param name="dispatcher"></param>
	/// <param name="log"></param>
	public LoadSettingsHandler(ISettingsRepository settings, IDispatcher dispatcher, ILog<LoadSettingsHandler> log) =>
		(Settings, Dispatcher, Log) = (settings, dispatcher, log);

	/// <summary>
	/// Retrieve the settings for user specified in <paramref name="query"/> - if none have been
	/// saved yet, returns a default object
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<Settings>> HandleAsync(LoadSettingsQuery query)
	{
		Log.Vrb("Load settings for User {UserId}", query.Id.Value);
		return Settings
			.StartFluentQuery()
			.Where(s => s.UserId, Compare.Equal, query.Id)
			.QuerySingleAsync<Settings>()
			.SwitchAsync(
				some: x => F.Some(x).AsTask(),
				none: _ => CreateSettings(query.Id)
			);
	}

	/// <summary>
	/// Create settings that do not yet exist for the specified user.
	/// </summary>
	/// <param name="userId">User ID</param>
	private Task<Maybe<Settings>> CreateSettings(AuthUserId userId) =>
		Dispatcher
			.DispatchAsync(new SaveSettingsCommand(userId, new()))
			.BindAsync(x => x switch
			{
				true =>
					Settings
						.StartFluentQuery()
						.Where(s => s.UserId, Compare.Equal, userId)
						.QuerySingleAsync<Settings>(),

				false =>
					F.None<Settings, FailedToCreateDefaultSettingsForUserMsg>().AsTask()
			});
}
