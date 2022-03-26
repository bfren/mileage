// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

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
	public override async Task<Maybe<bool>> HandleAsync(SaveSettingsCommand command)
	{
		// Ensure the car belongs to the user (or is null)
		var carBelongsToUser = await CheckCarBelongsToUser(
			command.Settings.DefaultCarId, command.UserId
		);

		// Ensure the place belongs to the user (or is null)
		var placeBelongsToUser = await CheckPlaceBelongsToUser(
			command.Settings.DefaultFromPlaceId, command.UserId
		);

		// If checks have failed, return with failure message
		if (!carBelongsToUser || !placeBelongsToUser)
		{
			return F.None<bool, SettingsCheckFailedMsg>();
		}

		// Add or update user settings
		return await Settings
			.StartFluentQuery()
			.Where(
				s => s.UserId, Compare.Equal, command.UserId
			)
			.QuerySingleAsync<SettingsEntity>()
			.SwitchAsync(
				some: x => Dispatcher.DispatchAsync(
					new Update.UpdateSettingsCommand(x, command.Settings)
				),
				none: () => Dispatcher.DispatchAsync(
					new Create.CreateSettingsCommand(command.UserId, command.Settings)
				)
			);
	}

	/// <summary>
	/// Returns true if <paramref name="carId"/> is null or belongs to <paramref name="userId"/>
	/// </summary>
	/// <param name="carId"></param>
	/// <param name="userId"></param>
	internal async Task<bool> CheckCarBelongsToUser(CarId? carId, AuthUserId userId) =>
		carId switch
		{
			CarId x =>
				await Dispatcher
					.DispatchAsync(
						new CheckCarBelongsToUserQuery(userId, x)
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
	internal async Task<bool> CheckPlaceBelongsToUser(PlaceId? placeId, AuthUserId userId) =>
		placeId switch
		{
			PlaceId x =>
				await Dispatcher
					.DispatchAsync(
						new CheckPlaceBelongsToUserQuery(userId, x)
					)
					.UnwrapAsync(
						x => x.Value(false)
					),

			_ =>
				true
		};
}
