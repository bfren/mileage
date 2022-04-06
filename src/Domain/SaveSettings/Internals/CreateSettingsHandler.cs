// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveSettings.Internals;

/// <summary>
/// Create settings for user
/// </summary>
internal sealed class CreateSettingsHandler : CommandHandler<CreateSettingsCommand>
{
	private ILog<CreateSettingsHandler> Log { get; init; }

	private ISettingsRepository Settings { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="settings"></param>
	/// <param name="log"></param>
	public CreateSettingsHandler(ISettingsRepository settings, ILog<CreateSettingsHandler> log) =>
		(Settings, Log) = (settings, log);

	/// <summary>
	/// Create settings for user specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(CreateSettingsCommand command)
	{
		Log.Vrb("Creating new settings for user {UserId}.", command.UserId.Value);
		return Settings
			.CreateAsync(new()
			{
				UserId = command.UserId,
				DefaultCarId = command.Settings.DefaultCarId,
				DefaultFromPlaceId = command.Settings.DefaultFromPlaceId
			})
			.AuditAsync(some: x => Log.Vrb("Created settings {SettingsId} for user {UserId}.", x.Value, command.UserId.Value))
			.BindAsync(_ => F.True);
	}
}
