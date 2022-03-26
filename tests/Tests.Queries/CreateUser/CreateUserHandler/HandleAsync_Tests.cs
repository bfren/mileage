// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Auth.Data.Entities;

namespace Mileage.Queries.CreateUser.CreateUserHandler_Tests;

public class HandleAsync_Tests : TestHandler<IAuthUserRepository, AuthUserEntity, AuthUserId, CreateUserHandler>
{
	public override CreateUserHandler GetHandler(Vars v) =>
		new(v.Repo, v.Log);

	[Fact]
	public async Task Logs_To_Vrb__With_Query_Using_Redacted_Password()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new CreateUserQuery(Rnd.Str, Rnd.Str, Rnd.Str);

		// Act
		await handler.HandleAsync(query, CancellationToken.None);

		// Assert
		v.Log.Received().Vrb("Create User: {Query}", query with { Password = "** REDACTED **" });
	}

	[Fact]
	public async void Calls_Repo_CreateAsync__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var name = Rnd.Str;
		var email = Rnd.Str;
		var password = Rnd.Str;
		var query = new CreateUserQuery(name, email, password);

		// Act
		await handler.HandleAsync(query, CancellationToken.None);

		// Assert
		await v.Repo.Received().CreateAsync(email, password, name);
	}

	[Fact]
	public async void Calls_Repo_CreateAsync__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var expected = new AuthUserId(Rnd.Lng);
		v.Repo.CreateAsync(email: default!, plainTextPassword: default!, friendlyName: default)
			.ReturnsForAnyArgs(expected);
		var query = new CreateUserQuery(Rnd.Str, Rnd.Str, Rnd.Str);

		// Act
		var result = await handler.HandleAsync(query, CancellationToken.None);

		// Assert
		var some = result.AssertSome();
		Assert.Equal(expected, some);
	}
}
