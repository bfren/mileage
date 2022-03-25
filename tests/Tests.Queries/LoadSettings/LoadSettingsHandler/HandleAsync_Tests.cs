// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Mileage.Persistence.Common;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Queries.LoadSettings.LoadSettingsHandler_Tests;

public class HandleAsync_Tests
{
	private static (ISettingsRepository, ILog<LoadSettingsHandler>, LoadSettingsHandler) Setup()
	{
		var repo = Substitute.For<ISettingsRepository>();
		var log = Substitute.For<ILog<LoadSettingsHandler>>();
		var handler = new LoadSettingsHandler(repo, log);

		return (repo, log, handler);
	}

	public class Calls_Log_Dbg
	{
		[Fact]
		public async Task With_Query()
		{
			// Arrange
			var (repo, log, handler) = Setup();
			repo.QuerySingleAsync<Settings>(predicates: default!)
				.ReturnsForAnyArgs(new Settings(Rnd.Lng, new(Rnd.Lng), new(Rnd.Lng)));
			var query = new LoadSettingsQuery(new(Rnd.Lng));

			// Act
			await handler.HandleAsync(query, CancellationToken.None);

			// Assert
			log.Received().Dbg("Load Settings for User {UserId}", query.Id.Value);
		}
	}

	public class Calls_Repo_QuerySingleAsync
	{
		[Fact]
		public async Task With_Correct_Values()
		{
			// Arrange
			var (repo, log, handler) = Setup();
			repo.QuerySingleAsync<Settings>(predicates: default!)
				.ReturnsForAnyArgs(new Settings(Rnd.Lng, new(Rnd.Lng), new(Rnd.Lng)));
			var userId = new AuthUserId(Rnd.Lng);
			var query = new LoadSettingsQuery(userId);

			// Act
			await handler.HandleAsync(query, CancellationToken.None);

			// Assert
			var calls = repo.ReceivedCalls();
			Assert.Collection(calls,
				c => Helpers.AssertQuery<SettingsEntity, Settings>(c,
					nameof(SettingsRepository.QuerySingleAsync),
					(x => x.UserId, Compare.Equal, userId)
				)
			);
		}

		[Fact]
		public async Task Returns_Result()
		{
			// Arrange
			var (repo, _, handler) = Setup();
			var model = new Settings(Rnd.Lng, new(Rnd.Lng), new(Rnd.Lng));
			repo.QuerySingleAsync<Settings>(predicates: default!)
				.ReturnsForAnyArgs(model);
			var query = new LoadSettingsQuery(new(Rnd.Lng));

			// Act
			var result = await handler.HandleAsync(query, CancellationToken.None);

			// Assert
			var some = result.AssertSome();
			Assert.Same(model, some);
		}
	}
}
