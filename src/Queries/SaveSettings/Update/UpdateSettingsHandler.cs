// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Persistence.Repositories;

namespace Mileage.Queries.SaveSettings.Update;

/// <summary>
/// Update settings for user
/// </summary>
internal class UpdateSettingsHandler : CommandHandler<UpdateSettingsCommand>
{
	private ILog<UpdateSettingsCommand> Log { get; init; }

	private ISettingsRepository Settings { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="settings"></param>
	/// <param name="log"></param>
	public UpdateSettingsHandler(ISettingsRepository settings, ILog<UpdateSettingsCommand> log) =>
		(Settings, Log) = (settings, log);

	/// <summary>
	/// Update settings for user specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(UpdateSettingsCommand command)
	{
		Log.Vrb("Updating settings {SettingsId} for user {UserId}.", command.ExistingSettings.Id.Value, command.ExistingSettings.UserId.Value);
		return Settings
			.UpdateAsync(command.ExistingSettings with
			{
				DefaultCarId = command.UpdatedSettings.DefaultCarId,
				DefaultFromPlaceId = command.UpdatedSettings.DefaultFromPlaceId
			});
	}
}
