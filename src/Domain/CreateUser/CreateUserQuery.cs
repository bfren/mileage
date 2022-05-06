// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;

namespace Mileage.Domain.CreateUser;

/// <inheritdoc cref="CreateUserHandler"/>
/// <param name="Name">User's name</param>
/// <param name="EmailAddress">Email address (for login)</param>
/// <param name="Password">Password (will be hashed)</param>
public sealed record class CreateUserQuery(
	string Name,
	string EmailAddress,
	string Password
) : Query<AuthUserId>;
