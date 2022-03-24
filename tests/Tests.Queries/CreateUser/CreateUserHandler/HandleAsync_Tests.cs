// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Logging;

namespace Mileage.Queries.CreateUser.CreateUserHandler_Tests;

public class HandleAsync_Tests
{
	private static (IAuthUserRepository, ILog<CreateUserHandler>, CreateUserHandler) Setup()
	{
		var repo = Substitute.For<IAuthUserRepository>();
		var log = Substitute.For<ILog<CreateUserHandler>>();
		var handler = new CreateUserHandler(repo, log);

		return (repo, log, handler);
	}

	public class Logs_To_Dbg
	{
		[Fact]
		public async Task With_Query_Using_Redacted_Password()
		{
			// Arrange
			var (_, log, handler) = Setup();
			var query = new CreateUserQuery(Rnd.Str, Rnd.Str, Rnd.Str);

			// Act
			await handler.HandleAsync(query, CancellationToken.None);

			// Assert
			log.Received().Dbg("Create User: {Query}", query with { Password = "** REDACTED **" });
		}
	}

	public class Calls_Repo_CreateAsync
	{
		[Fact]
		public async void With_Correct_Values()
		{
			// Arrange
			var (repo, _, handler) = Setup();
			var name = Rnd.Str;
			var email = Rnd.Str;
			var password = Rnd.Str;
			var query = new CreateUserQuery(name, email, password);

			// Act
			await handler.HandleAsync(query, CancellationToken.None);

			// Assert
			await repo.Received().CreateAsync(email, password, name);
		}

		[Fact]
		public async void Returns_Result()
		{
			// Arrange
			var (repo, _, handler) = Setup();
			var expected = new AuthUserId(Rnd.Lng);
			repo.CreateAsync(email: default!, plainTextPassword: default!, friendlyName: default)
				.ReturnsForAnyArgs(expected);
			var query = new CreateUserQuery(Rnd.Str, Rnd.Str, Rnd.Str);

			// Act
			var result = await handler.HandleAsync(query, CancellationToken.None);

			// Assert
			var some = result.AssertSome();
			Assert.Equal(expected, some);
		}
	}
}
