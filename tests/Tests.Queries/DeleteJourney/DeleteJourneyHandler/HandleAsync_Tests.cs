// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Jeebs.Data.Query;
using Jeebs.Logging;
using Jeebs.Messages;
using MaybeF;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;
using Mileage.Queries.DeleteJourney.Messages;

namespace Mileage.Queries.DeleteJourney.DeleteJourneyHandler_Tests;

public class HandleAsync_Tests
{
	private static (IJourneyRepository, IQueryFluent<JourneyEntity, JourneyId>, ILog<DeleteJourneyHandler>, DeleteJourneyHandler) Setup()
	{
		var (repo, fluent, log) = Helpers.Setup<IJourneyRepository, JourneyEntity, JourneyId, DeleteJourneyHandler, DeleteJourneyQuery, bool>();
		var handler = new DeleteJourneyHandler(repo, log);

		return (repo, fluent, log, handler);
	}

	public class Calls_Log_Vrb
	{
		[Fact]
		public async Task With_Query()
		{
			// Arrange
			var (_, repo, log, handler) = Setup();
			repo.QuerySingleAsync<JourneyToDelete>()
				.Returns(new JourneyToDelete(new(Rnd.Lng), Rnd.Lng));
			var query = new DeleteJourneyQuery(new(), new());

			// Act
			await handler.HandleAsync(query, CancellationToken.None);

			// Assert
			log.Received().Vrb("Delete Journey: {Query}", query);
		}
	}

	public class Calls_FluentQuery_Where
	{
		[Fact]
		public async Task With_Correct_Values()
		{
			// Arrange
			var (_, fluent, _, handler) = Setup();
			fluent.QuerySingleAsync<JourneyToDelete>()
				.Returns(new JourneyToDelete(new(Rnd.Lng), Rnd.Lng));
			var journeyId = new JourneyId(Rnd.Lng);
			var userId = new AuthUserId(Rnd.Lng);
			var query = new DeleteJourneyQuery(journeyId, userId);

			// Act
			await handler.HandleAsync(query, CancellationToken.None);

			// Assert
			var calls = fluent.ReceivedCalls();
			Assert.Collection(calls,
				c => Helpers.AssertWhere<JourneyEntity, JourneyId>(c, x => x.Id, Compare.Equal, journeyId),
				c => Helpers.AssertWhere<JourneyEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
				_ => { }
			);
		}
	}

	public class Calls_FluentQuery_WhereSingleAsync
	{
		public class Receives_None
		{
			[Fact]
			public async Task Audits_Msg()
			{
				// Arrange
				var (_, fluent, log, handler) = Setup();
				var msg = new TestMsg();
				fluent.QuerySingleAsync<JourneyToDelete>()
					.Returns(F.None<JourneyToDelete>(msg));
				var query = new DeleteJourneyQuery(new(), new());

				// Act
				await handler.HandleAsync(query, CancellationToken.None);

				// Assert
				log.Received().Msg(msg);
			}

			[Fact]
			public async Task Returns_None_With_JourneyDoesNotExistMsg()
			{
				// Arrange
				var (_, fluent, _, handler) = Setup();
				fluent.QuerySingleAsync<JourneyToDelete>()
					.Returns(Create.None<JourneyToDelete>());
				var journeyId = new JourneyId(Rnd.Lng);
				var userId = new AuthUserId(Rnd.Lng);
				var query = new DeleteJourneyQuery(journeyId, userId);

				// Act
				var result = await handler.HandleAsync(query, CancellationToken.None);

				// Assert
				var none = result.AssertNone();
				var msg = Assert.IsType<JourneyDoesNotExistMsg>(none);
				Assert.Equal(journeyId, msg.JourneyId);
				Assert.Equal(userId, msg.UserId);
			}
		}

		public class Receives_Some
		{
			public class Calls_Repo_DeleteAsync
			{
				[Fact]
				public async Task With_Correct_Value()
				{
					// Arrange
					var (repo, fluent, _, handler) = Setup();
					var journeyId = new JourneyId(Rnd.Lng);
					var userId = new AuthUserId(Rnd.Lng);
					var query = new DeleteJourneyQuery(journeyId, userId);
					var model = new JourneyToDelete(journeyId, Rnd.Lng);
					fluent.QuerySingleAsync<JourneyToDelete>()
						.Returns(model);

					// Act
					await handler.HandleAsync(query, CancellationToken.None);

					// Assert
					await repo.Received().DeleteAsync(model);
				}

				[Fact]
				public async Task Returns_Result()
				{
					// Arrange
					var (repo, fluent, _, handler) = Setup();
					var model = new JourneyToDelete(new(Rnd.Lng), Rnd.Lng);
					fluent.QuerySingleAsync<JourneyToDelete>()
						.Returns(model);
					var expected = Rnd.Flip;
					repo.DeleteAsync<JourneyToDelete>(default!)
						.ReturnsForAnyArgs(expected);
					var journeyId = new JourneyId(Rnd.Lng);
					var userId = new AuthUserId(Rnd.Lng);
					var query = new DeleteJourneyQuery(journeyId, userId);

					// Act
					var result = await handler.HandleAsync(query, CancellationToken.None);

					// Assert
					var some = result.AssertSome();
					Assert.Equal(expected, some);
				}
			}
		}
	}

	public sealed record class TestMsg : Msg;
}
