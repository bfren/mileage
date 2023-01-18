// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Logging;

namespace Mileage.Domain.GetUserProfile.GetUserProfileHandler_Tests;

public sealed class HandleAsync_Tests
{
	[Fact]
	public async Task Calls_Log_Vrb__With_Correct_Values()
	{
		// Arrange
		var auth = Substitute.For<IAuthDataProvider>();
		var log = Substitute.For<ILog<GetUserProfileHandler>>();
		var command = new GetUserProfileQuery(LongId<AuthUserId>());
		var handler = new GetUserProfileHandler(auth, log);

		// Act
		_ = await handler.HandleAsync(command);

		// Assert
		log.Received().Vrb("Getting profile for User {User}.", command.Id.Value);
	}

	[Fact]
	public async Task Calls_User_UpdateAsync__With_Correct_Values()
	{
		// Arrange
		var user = Substitute.For<IAuthUserRepository>();
		var auth = Substitute.For<IAuthDataProvider>();
		auth.User
			.Returns(user);
		var log = Substitute.For<ILog<GetUserProfileHandler>>();
		var query = new GetUserProfileQuery(LongId<AuthUserId>());
		var handler = new GetUserProfileHandler(auth, log);

		// Act
		_ = await handler.HandleAsync(query);

		// Assert
		await user.Received().RetrieveAsync<UserProfileModel>(query.Id);
	}
}
