// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Domain.CheckRateBelongsToUser;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveSettings;

/// <summary>
/// Update default rate
/// </summary>
internal sealed class UpdateDefaultRateHandler : CommandHandler<UpdateDefaultRateCommand>
{
	private IDispatcher Dispatcher { get; init; }

	private ISettingsRepository Settings { get; init; }

	private ILog<UpdateDefaultRateHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="dispatcher"></param>
	/// <param name="settings"></param>
	/// <param name="log"></param>
	public UpdateDefaultRateHandler(IDispatcher dispatcher, ISettingsRepository settings, ILog<UpdateDefaultRateHandler> log) =>
		(Dispatcher, Settings, Log) = (dispatcher, settings, log);

	/// <summary>
	/// Update default rate for user specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override async Task<Maybe<bool>> HandleAsync(UpdateDefaultRateCommand command)
	{
		if (command.DefaultRateId is not null)
		{
			var check = await Dispatcher.DispatchAsync(new CheckRateBelongsToUserQuery(command.UserId, command.DefaultRateId));
			if (!check.IsSome(out var value) || !value)
			{
				return F.None<bool, Messages.SaveSettingsCheckFailedMsg>();
			}
		}

		Log.Vrb("Updating Default Rate for {User}.", command.UserId.Value);
		return await Settings.UpdateAsync(command);
	}
}
