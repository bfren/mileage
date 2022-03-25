// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading;
using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;
using Mileage.Queries.CheckCarBelongsToUser;
using Mileage.Queries.CheckPlaceBelongsToUser;
using Mileage.Queries.SaveSettings.Messages;

namespace Mileage.Queries.SaveSettings;

/// <summary>
/// Save settings for a user
/// </summary>
public sealed class SaveSettingsHandler : CommandHandler<SaveSettingsCommand>
{
	private IDispatcher Dispatcher { get; init; }

	private ILog<SaveSettingsHandler> Log { get; init; }

	private ISettingsRepository Settings { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="dispatcher"></param>
	/// <param name="settings"></param>
	/// <param name="log"></param>
	public SaveSettingsHandler(IDispatcher dispatcher, ISettingsRepository settings, ILog<SaveSettingsHandler> log) =>
		(Dispatcher, Settings, Log) = (dispatcher, settings, log);

	/// <summary>
	/// Save the settings for user specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	/// <param name="cancellationToken"></param>
	public override async Task<Maybe<bool>> HandleAsync(SaveSettingsCommand command, CancellationToken cancellationToken)
	{
		// Ensure the car belongs to the user (or is null)
		var carBelongsToUser = await CheckCarBelongsToUser(
			command.Settings.DefaultCarId, command.UserId, cancellationToken
		);

		// Ensure the place belongs to the user (or is null)
		var placeBelongsToUser = await CheckPlaceBelongsToUser(
			command.Settings.DefaultFromPlaceId, command.UserId, cancellationToken
		);

		// If checks have failed, return with failure message
		if (!carBelongsToUser || !placeBelongsToUser)
		{
			return F.None<bool, SettingsCheckFailedMsg>();
		}

		// Add or update user settings
		var settings = await Settings
			.StartFluentQuery()
			.Where(
				s => s.UserId, Compare.Equal, command.UserId
			)
			.QuerySingleAsync<SettingsEntity>();

		// Update settings
		if (settings.IsSome(out var s))
		{
			Log.Vrb("Updating settings {SettingsId} for user {UserId}.", s.Id.Value, command.UserId.Value);
			return await Settings
				.UpdateAsync(s with
				{
					DefaultCarId = command.Settings.DefaultCarId,
					DefaultFromPlaceId = command.Settings.DefaultFromPlaceId
				});
		}

		// Add settings
		Log.Vrb("Creating new settings for user {UserId}.", command.UserId.Value);
		return await Settings
			.CreateAsync(new()
			{
				UserId = command.UserId,
				DefaultCarId = command.Settings.DefaultCarId,
				DefaultFromPlaceId = command.Settings.DefaultFromPlaceId
			})
			.AuditAsync(
				some: x => Log.Vrb("Created settings {SettingsId} for user {UserId}.", x.Value, command.UserId.Value)
			)
			.BindAsync(
				_ => F.True
			);
	}

	/// <summary>
	/// Returns true if <paramref name="carId"/> is null or belongs to <paramref name="userId"/>
	/// </summary>
	/// <param name="carId"></param>
	/// <param name="userId"></param>
	/// <param name="cancellationToken"></param>
	internal async Task<bool> CheckCarBelongsToUser(CarId? carId, AuthUserId userId, CancellationToken cancellationToken) =>
		carId switch
		{
			CarId x =>
				await Dispatcher
					.DispatchAsync(
						new CheckCarBelongsToUserQuery(userId, x), cancellationToken
					)
					.UnwrapAsync(
						x => x.Value(false)
					),

			_ =>
				true
		};

	/// <summary>
	/// Returns true if <paramref name="placeId"/> is null or belongs to <paramref name="userId"/>
	/// </summary>
	/// <param name="placeId"></param>
	/// <param name="userId"></param>
	/// <param name="cancellationToken"></param>
	internal async Task<bool> CheckPlaceBelongsToUser(PlaceId? placeId, AuthUserId userId, CancellationToken cancellationToken) =>
		placeId switch
		{
			PlaceId x =>
				await Dispatcher
					.DispatchAsync(
						new CheckPlaceBelongsToUserQuery(userId, x), cancellationToken
					)
					.UnwrapAsync(
						x => x.Value(false)
					),

			_ =>
				true
		};
}
