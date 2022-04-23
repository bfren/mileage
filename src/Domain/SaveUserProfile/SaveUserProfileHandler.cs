// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Logging;

namespace Mileage.Domain.SaveUserProfile;

/// <summary>
/// Save user profile
/// </summary>
internal sealed class SaveUserProfileHandler : CommandHandler<SaveUserProfileCommand>
{
	private IAuthDataProvider Auth { get; }

	private ILog<SaveUserProfileHandler> Log { get; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="auth"></param>
	/// <param name="log"></param>
	public SaveUserProfileHandler(IAuthDataProvider auth, ILog<SaveUserProfileHandler> log) =>
		(Auth, Log) = (auth, log);

	/// <summary>
	/// Save profile for user specified in <paramref name="command"/>
	/// </summary>
	/// <param name="command"></param>
	public override Task<Maybe<bool>> HandleAsync(SaveUserProfileCommand command)
	{
		Log.Vrb("Saving profile for User {User}.", command.Id.Value);
		return Auth.User.UpdateAsync(command);
	}
}
