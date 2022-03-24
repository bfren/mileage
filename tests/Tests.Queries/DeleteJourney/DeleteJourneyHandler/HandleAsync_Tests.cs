// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Jeebs.Logging;
using Jeebs.Messages;
using MaybeF;
using Persistence.Entities;
using Persistence.Entities.StrongIds;
using Persistence.Repositories;
using Queries.DeleteJourney.Messages;

namespace Queries.DeleteJourney.DeleteJourneyHandler_Tests;

public class HandleAsync_Tests
{
	private static (IJourneyRepository, ILog<DeleteJourneyHandler>, DeleteJourneyHandler) Setup()
	{
		var repo = Substitute.For<IJourneyRepository>();
		var log = Substitute.For<ILog<DeleteJourneyHandler>>();
		var handler = new DeleteJourneyHandler(repo, log);

		return (repo, log, handler);
	}

	public class Calls_Log_Dbg
	{
		[Fact]
		public async Task With_Query()
		{
			// Arrange
			var (repo, log, handler) = Setup();
			repo.QuerySingleAsync<JourneyToDelete>(default!).ReturnsForAnyArgs(
				new JourneyToDelete(new(Rnd.Lng))
			);
			var query = new DeleteJourneyQuery(new(), new());

			// Act
			await handler.HandleAsync(query, CancellationToken.None);

			// Assert
			log.Received().Dbg("Delete Journey: {Query}", query);
		}
	}

	public class Calls_Repo_QuerySingleAsync
	{
		[Fact]
		public async Task With_Correct_Values()
		{
			// Arrange
			var (repo, log, handler) = Setup();
			repo.QuerySingleAsync<JourneyToDelete>(default!).ReturnsForAnyArgs(
				new JourneyToDelete(new(Rnd.Lng))
			);
			var journeyId = new JourneyId(Rnd.Lng);
			var userId = new AuthUserId(Rnd.Lng);
			var query = new DeleteJourneyQuery(journeyId, userId);

			// Act
			await handler.HandleAsync(query, CancellationToken.None);

			// Assert
			var calls = repo.ReceivedCalls();
			Assert.Collection(calls,
				c => Helpers.AssertQuery<JourneyEntity, JourneyToDelete>(c,
					nameof(JourneyRepository.QuerySingleAsync),
					(x => x.Id, Compare.Equal, journeyId),
					(x => x.UserId, Compare.Equal, userId)
				),
				_ => { }
			);
		}

		public class Receives_None
		{
			[Fact]
			public async Task Audits_Msg()
			{
				// Arrange
				var (repo, log, handler) = Setup();
				var msg = new TestMsg();
				repo.QuerySingleAsync<JourneyToDelete>(default!).ReturnsForAnyArgs(
					F.None<JourneyToDelete>(msg)
				);
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
				var (repo, _, handler) = Setup();
				repo.QuerySingleAsync<JourneyToDelete>(default!).ReturnsForAnyArgs(
					Create.None<JourneyToDelete>()
				);
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
					var journeyId = new JourneyId(Rnd.Lng);
					var userId = new AuthUserId(Rnd.Lng);
					var query = new DeleteJourneyQuery(journeyId, userId);
					var (repo, _, handler) = Setup();
					repo.QuerySingleAsync<JourneyToDelete>(default!).ReturnsForAnyArgs(
						new JourneyToDelete(journeyId)
					);

					// Act
					await handler.HandleAsync(query, CancellationToken.None);

					// Assert
					await repo.Received().DeleteAsync(journeyId);
				}

				[Fact]
				public async Task Returns_Result()
				{
					// Arrange
					var (repo, _, handler) = Setup();
					repo.QuerySingleAsync<JourneyToDelete>(default!).ReturnsForAnyArgs(
						new JourneyToDelete(new(Rnd.Lng))
					);
					var expected = Rnd.Flip;
					repo.DeleteAsync(default).ReturnsForAnyArgs(
						expected
					);
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
