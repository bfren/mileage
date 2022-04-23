// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Logging;

namespace Mileage.Domain.SaveUserPassword;

/// <summary>
/// Save user password
/// </summary>
internal sealed class SaveUserPasswordHandler : CommandHandler<SaveUserPasswordCommand>
{
	private IAuthDataProvider Auth { get; }

	private ILog<SaveUserPasswordHandler> Log { get; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="auth"></param>
	/// <param name="log"></param>
	public SaveUserPasswordHandler(IAuthDataProvider auth, ILog<SaveUserPasswordHandler> log) =>
		(Auth, Log) = (auth, log);

	/// <summary>
	/// Save password for user specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(SaveUserPasswordCommand command)
	{
		Log.Vrb("Saving password for User {User}.", command.Id.Value);
		return Auth.ChangeUserPasswordAsync(new(
			Id: command.Id,
			Version: command.Version,
			CurrentPassword: command.CurrentPassword,
			NewPassword: command.NewPassword,
			CheckPassword: command.CheckPassword
		));
	}
}
