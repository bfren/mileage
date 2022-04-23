// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Logging;

namespace Mileage.Domain.GetUserProfile;

/// <summary>
/// Get user profile
/// </summary>
internal sealed class GetUserProfileHandler : QueryHandler<GetUserProfileQuery, UserProfileModel>
{
	private IAuthDataProvider Auth { get; }

	private ILog<GetUserProfileHandler> Log { get; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="auth"></param>
	/// <param name="log"></param>
	public GetUserProfileHandler(IAuthDataProvider auth, ILog<GetUserProfileHandler> log) =>
		(Auth, Log) = (auth, log);

	/// <summary>
	/// Retrieve profile for user specified in <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	public override Task<Maybe<UserProfileModel>> HandleAsync(GetUserProfileQuery query)
	{
		Log.Vrb("Getting profile for User {User}.", query.Id.Value);
		return Auth.User.RetrieveAsync<UserProfileModel>(query.Id);
	}
}
