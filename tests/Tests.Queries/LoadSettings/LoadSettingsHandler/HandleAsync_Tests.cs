// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Jeebs.Data.Query;
using Jeebs.Logging;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Queries.LoadSettings.LoadSettingsHandler_Tests;

public class HandleAsync_Tests
{
	private static (IQueryFluent<SettingsEntity, SettingsId>, ILog<LoadSettingsHandler>, LoadSettingsHandler) Setup()
	{
		var (repo, fluent, log) = Helpers.Setup<ISettingsRepository, SettingsEntity, SettingsId, LoadSettingsHandler, LoadSettingsQuery, Settings>();
		var handler = new LoadSettingsHandler(repo, log);

		return (fluent, log, handler);
	}

	public class Calls_Log_Vrb
	{
		[Fact]
		public async Task With_Query()
		{
			// Arrange
			var (fluent, log, handler) = Setup();
			fluent.QuerySingleAsync<Settings>()
				.Returns(new Settings(new(Rnd.Lng), Rnd.Lng, new(Rnd.Lng), new(Rnd.Lng)));
			var query = new LoadSettingsQuery(new(Rnd.Lng));

			// Act
			await handler.HandleAsync(query, CancellationToken.None);

			// Assert
			log.Received().Vrb("Load settings for User {UserId}", query.Id.Value);
		}
	}

	public class Calls_FluentQuery_Where
	{
		[Fact]
		public async Task With_Correct_Values()
		{
			// Arrange
			var (fluent, _, handler) = Setup();
			fluent.QuerySingleAsync<Settings>()
				.Returns(new Settings(new(Rnd.Lng), Rnd.Lng, new(Rnd.Lng), new(Rnd.Lng)));
			var userId = new AuthUserId(Rnd.Lng);
			var query = new LoadSettingsQuery(userId);

			// Act
			await handler.HandleAsync(query, CancellationToken.None);

			// Assert
			var calls = fluent.ReceivedCalls();
			Assert.Collection(calls,
				c => Helpers.AssertWhere<SettingsEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
				_ => { }
			);
		}
	}

	public class Calls_FluentQuery_QuerySingleAsync
	{
		public class Receives_Some
		{
			[Fact]
			public async Task Returns_Result()
			{
				// Arrange
				var (fluent, _, handler) = Setup();
				var model = new Settings(new(Rnd.Lng), Rnd.Lng, new(Rnd.Lng), new(Rnd.Lng));
				fluent.QuerySingleAsync<Settings>()
					.Returns(model);
				var query = new LoadSettingsQuery(new(Rnd.Lng));

				// Act
				var result = await handler.HandleAsync(query, CancellationToken.None);

				// Assert
				var some = result.AssertSome();
				Assert.Same(model, some);
			}
		}

		public class Receives_None
		{
			[Fact]
			public async Task Returns_Default_Settings()
			{
				// Arrange
				var (fluent, _, handler) = Setup();
				fluent.QuerySingleAsync<Settings>()
					.Returns(Create.None<Settings>());
				var query = new LoadSettingsQuery(new(Rnd.Lng));

				// Act
				var result = await handler.HandleAsync(query, CancellationToken.None);

				// Assert
				var some = result.AssertSome();
				Assert.Equal(new(), some);
			}
		}
	}
}
