// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading;
using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Logging;

namespace Mileage.Queries.CreateUser;

/// <summary>
/// Create a new user
/// </summary>
public sealed class CreateUserHandler : QueryHandler<CreateUserQuery, AuthUserId>
{
	private ILog<CreateUserHandler> Log { get; init; }

	private IAuthUserRepository User { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="user"></param>
	/// <param name="log"></param>
	public CreateUserHandler(IAuthUserRepository user, ILog<CreateUserHandler> log) =>
		(User, Log) = (user, log);

	/// <summary>
	/// Create a new user from <paramref name="query"/>
	/// </summary>
	/// <param name="query"></param>
	/// <param name="cancellationToken"></param>
	public override Task<Maybe<AuthUserId>> HandleAsync(CreateUserQuery query, CancellationToken cancellationToken)
	{
		Log.Dbg("Create User: {Query}", query with { Password = "** REDACTED **" });
		return User.CreateAsync(query.EmailAddress, query.Password, query.Name);
	}
}
