// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Domain.CheckPlacesBelongToUser;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveSettings;

/// <summary>
/// Update default from place
/// </summary>
internal sealed class UpdateDefaultFromPlaceHandler : CommandHandler<UpdateDefaultFromPlaceCommand>
{
	private IDispatcher Dispatcher { get; init; }

	private ISettingsRepository Settings { get; init; }

	private ILog<UpdateDefaultFromPlaceHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="dispatcher"></param>
	/// <param name="settings"></param>
	/// <param name="log"></param>
	public UpdateDefaultFromPlaceHandler(IDispatcher dispatcher, ISettingsRepository settings, ILog<UpdateDefaultFromPlaceHandler> log) =>
		(Dispatcher, Settings, Log) = (dispatcher, settings, log);

	/// <summary>
	/// Update default car for user specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override async Task<Maybe<bool>> HandleAsync(UpdateDefaultFromPlaceCommand command)
	{
		if (command.DefaultFromPlaceId is not null)
		{
			var check = await Dispatcher.DispatchAsync(new CheckPlacesBelongToUserQuery(command.UserId, command.DefaultFromPlaceId));
			if (!check.IsSome(out var value) || !value)
			{
				return F.None<bool, Messages.SaveSettingsCheckFailedMsg>();
			}
		}

		Log.Vrb("Updating Default From Place for {User}.", command.UserId.Value);
		return await Settings.UpdateAsync(command);
	}
}
