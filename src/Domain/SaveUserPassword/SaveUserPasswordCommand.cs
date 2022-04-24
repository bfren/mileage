// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Data;

namespace Mileage.Domain.SaveUserPassword;

/// <inheritdoc cref="SaveUserPasswordHandler"/>
/// <param name="Id"></param>
/// <param name="Version"></param>
/// <param name="CurrentPassword"></param>
/// <param name="NewPassword"></param>
/// <param name="CheckPassword"></param>
public sealed record class SaveUserPasswordCommand(
	AuthUserId Id,
	long Version,
	string CurrentPassword,
	string NewPassword,
	string CheckPassword
) : ICommand, IWithVersion<AuthUserId>;
