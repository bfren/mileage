// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Domain.CheckCarBelongsToUser;
using Mileage.Domain.CheckPlacesBelongToUser;
using Mileage.Domain.SaveSettings.Messages;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveSettings;

/// <summary>
/// Save settings for a user - create if they don't exist, or update if they do
/// </summary>
internal sealed class SaveSettingsHandler : CommandHandler<SaveSettingsCommand>
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
			return F.None<bool, SaveSettingsCheckFailedMsg>();
		}

		// Add or update user settings
		return await Settings
			.StartFluentQuery()
			.Where(s => s.UserId, Compare.Equal, command.UserId)
			.QuerySingleAsync<SettingsEntity>()
			.SwitchAsync(
				some: x => Dispatcher
					.DispatchAsync(new Internals.UpdateSettingsCommand(x, command.Settings)),
				none: () => Dispatcher
					.DispatchAsync(new Internals.CreateSettingsCommand(command.UserId, command.Settings))
			);
	}

	/// <summary>
	/// Returns true if <paramref name="carId"/> is null or belongs to <paramref name="userId"/>
	/// </summary>
	/// <param name="carId"></param>
	/// <param name="userId"></param>
	internal Task<bool> CheckCarBelongsToUser(CarId? carId, AuthUserId userId) =>
		carId switch
		{
			CarId x =>
				Dispatcher
					.DispatchAsync(new CheckCarBelongsToUserQuery(userId, x))
					.IsTrueAsync(),

			_ =>
				Task.FromResult(true)
		};

	/// <summary>
	/// Returns true if <paramref name="placeId"/> is null or belongs to <paramref name="userId"/>
	/// </summary>
	/// <param name="placeId"></param>
	/// <param name="userId"></param>
	internal Task<bool> CheckPlaceBelongsToUser(PlaceId? placeId, AuthUserId userId) =>
		placeId switch
		{
			PlaceId x =>
				Dispatcher
					.DispatchAsync(new CheckPlacesBelongToUserQuery(userId, x))
					.IsTrueAsync(),

			_ =>
				Task.FromResult(true)
		};
}
