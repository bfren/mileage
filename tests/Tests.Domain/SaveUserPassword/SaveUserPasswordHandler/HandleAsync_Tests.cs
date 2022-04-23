// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Auth.Data.Models;
using Jeebs.Logging;

namespace Mileage.Domain.SaveUserPassword.SaveUserPassword_Tests;

public class HandleAsync_Tests
{
	[Fact]
	public async Task Calls_Log_Vrb__With_Correct_Values()
	{
		// Arrange
		var auth = Substitute.For<IAuthDataProvider>();
		var log = Substitute.For<ILog<SaveUserPasswordHandler>>();
		var command = new SaveUserPasswordCommand(LongId<AuthUserId>(), Rnd.Lng, Rnd.Str, Rnd.Str, Rnd.Str);
		var handler = new SaveUserPasswordHandler(auth, log);

		// Act
		_ = await handler.HandleAsync(command);

		// Assert
		log.Received().Vrb("Saving password for User {User}.", command.Id.Value);
	}

	[Fact]
	public async Task Calls_User_UpdateAsync__With_Correct_Values()
	{
		// Arrange
		var auth = Substitute.For<IAuthDataProvider>();
		var log = Substitute.For<ILog<SaveUserPasswordHandler>>();
		var command = new SaveUserPasswordCommand(LongId<AuthUserId>(), Rnd.Lng, Rnd.Str, Rnd.Str, Rnd.Str);
		var handler = new SaveUserPasswordHandler(auth, log);

		// Act
		_ = await handler.HandleAsync(command);

		// Assert
		await auth.Received().ChangeUserPasswordAsync(Arg.Is<AuthChangePasswordModel>(x =>
			x.Id == command.Id
			&& x.Version == command.Version
			&& x.CurrentPassword == command.CurrentPassword
			&& x.NewPassword == command.NewPassword
			&& x.CheckPassword == command.CheckPassword
		));
	}
}
