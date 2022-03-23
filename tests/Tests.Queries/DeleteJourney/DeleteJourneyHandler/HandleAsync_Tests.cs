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

namespace Queries.DeleteJourney.DeleteJourneyHandler_Tests;

public class HandleAsync_Tests
{
	private (IJourneyRepository, ILog<DeleteJourneyHandler>, DeleteJourneyHandler) Setup()
	{
		var repo = Substitute.For<IJourneyRepository>();
		var log = Substitute.For<ILog<DeleteJourneyHandler>>();
		var handler = new DeleteJourneyHandler(repo, log);

		return (repo, log, handler);
	}

	[Fact]
	public async Task Logs_To_Dbg()
	{
		// Arrange
		var (repo, log, handler) = Setup();
		repo.QuerySingleAsync<DeleteJourneyHandler.JourneyToDelete>(default!).ReturnsForAnyArgs(
			new DeleteJourneyHandler.JourneyToDelete(new(Rnd.Lng))
		);
		var query = new DeleteJourneyQuery(new(), new());

		// Act
		await handler.HandleAsync(query, CancellationToken.None);

		// Assert
		log.Received().Dbg("Delete Journey: {Query}", query);
	}

	[Fact]
	public async Task Calls_Repo_QuerySingleAsync_With_Correct_Values()
	{
		// Arrange
		var (repo, log, handler) = Setup();
		repo.QuerySingleAsync<DeleteJourneyHandler.JourneyToDelete>(default!).ReturnsForAnyArgs(
			new DeleteJourneyHandler.JourneyToDelete(new(Rnd.Lng))
		);
		var journeyId = new JourneyId(Rnd.Lng);
		var userId = new AuthUserId(Rnd.Lng);
		var query = new DeleteJourneyQuery(journeyId, userId);

		// Act
		await handler.HandleAsync(query, CancellationToken.None);

		// Assert
		var calls = repo.ReceivedCalls();
		Assert.Collection(calls,
			c => Helpers.AssertQuery<JourneyEntity, DeleteJourneyHandler.JourneyToDelete>(c,
				nameof(JourneyRepository.QuerySingleAsync),
				(x => x.Id, Compare.Equal, journeyId),
				(x => x.UserId, Compare.Equal, userId)
			),
			_ => { }
		);
	}

	[Fact]
	public async Task Repo_QuerySingleAsync_Returns_None_Audits_Msg()
	{
		// Arrange
		var (repo, log, handler) = Setup();
		var msg = new TestMsg();
		repo.QuerySingleAsync<DeleteJourneyHandler.JourneyToDelete>(default!).ReturnsForAnyArgs(
			F.None<DeleteJourneyHandler.JourneyToDelete>(msg)
		);
		var query = new DeleteJourneyQuery(new(), new());

		// Act
		await handler.HandleAsync(query, CancellationToken.None);

		// Assert
		log.Received().Msg(msg);
	}

	public sealed record class TestMsg : Msg;
}
