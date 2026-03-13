// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Domain.CheckCarBelongsToUser;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveSettings;

/// <summary>
/// Update default car
/// </summary>
internal sealed class UpdateDefaultCarHandler : CommandHandler<UpdateDefaultCarCommand>
{
	private IDispatcher Dispatcher { get; init; }

	private ISettingsRepository Settings { get; init; }

	private ILog<UpdateDefaultCarHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="dispatcher"></param>
	/// <param name="settings"></param>
	/// <param name="log"></param>
	public UpdateDefaultCarHandler(IDispatcher dispatcher, ISettingsRepository settings, ILog<UpdateDefaultCarHandler> log) =>
		(Dispatcher, Settings, Log) = (dispatcher, settings, log);

	/// <summary>
	/// Update default car for user specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override async Task<Result<bool>> HandleAsync(UpdateDefaultCarCommand command)
	{
		if (command.DefaultCarId?.Value == 0)
		{
			command = command with { DefaultCarId = null };
		}

		if (command.DefaultCarId is not null)
		{
			var check = await Dispatcher.SendAsync(new CheckCarBelongsToUserQuery(command.UserId, command.DefaultCarId));
			if (check.Unsafe().TryFailure(out var _) || (check.Unsafe().TryOk(out var value) && !value))
			{
				return R.Fail("Save settings check failed.")
					.Ctx(nameof(UpdateDefaultCarHandler), nameof(HandleAsync));
			}
		}

		Log.Vrb("Updating Default Car for {User}.", command.UserId.Value);
		return await Settings.UpdateAsync(command);
	}
}
