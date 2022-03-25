// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading;
using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Common;
using Mileage.Persistence.Repositories;

namespace Mileage.Queries.LoadSettings;

/// <summary>
/// Load a user's settings
/// </summary>
public sealed class LoadSettingsHandler : QueryHandler<LoadSettingsQuery, Settings>
{
	private ILog<LoadSettingsHandler> Log { get; init; }

	private ISettingsRepository Settings { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="settings"></param>
	/// <param name="log"></param>
	public LoadSettingsHandler(ISettingsRepository settings, ILog<LoadSettingsHandler> log) =>
		(Settings, Log) = (settings, log);

	/// <summary>
	/// Retrieve the settings for user specified in <paramref name="query"/> - if none have been
	/// saved yet, returns a default object
	/// </summary>
	/// <param name="query"></param>
	/// <param name="cancellationToken"></param>
	public override Task<Maybe<Settings>> HandleAsync(LoadSettingsQuery query, CancellationToken cancellationToken)
	{
		Log.Dbg("Load Settings for User {UserId}", query.Id.Value);
		return Settings.QuerySingleAsync<Settings>(
			(s => s.UserId, Compare.Equal, query.Id)
		).SwitchAsync(
			x => F.Some(x).AsTask,
			_ => F.Some(new Settings()).AsTask
		);
	}
}
