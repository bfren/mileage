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

namespace Mileage.Queries.CheckCarBelongsToUser.CheckCarBelongsToUserHandler_Tests;

public class HandleAsync_Tests
{
	private static (ICarRepository, IQueryFluent<CarEntity, CarId>, ILog<CheckCarBelongsToUserHandler>, CheckCarBelongsToUserHandler) Setup()
	{
		var (repo, fluent, log) = Helpers.Setup<ICarRepository, CarEntity, CarId, CheckCarBelongsToUserHandler, CheckCarBelongsToUserQuery, bool>();
		var handler = new CheckCarBelongsToUserHandler(repo, log);

		return (repo, fluent, log, handler);
	}

	public class Calls_Log_Vrb
	{
		[Fact]
		public async Task With_Query()
		{
			// Arrange
			var (_, fluent, log, handler) = Setup();
			fluent.QuerySingleAsync<CarEntity>()
				.Returns(new CarEntity());
			var query = new CheckCarBelongsToUserQuery(new(), new());

			// Act
			await handler.HandleAsync(query, CancellationToken.None);

			// Assert
			log.Received().Vrb("Checking car {CarId} belongs to user {UserId}.", query.CarId.Value, query.UserId.Value);
		}
	}

	public class Calls_FluentQuery_Where
	{
		[Fact]
		public async Task With_Correct_Values()
		{
			// Arrange
			var (_, fluent, _, handler) = Setup();
			fluent.QuerySingleAsync<CarEntity>()
				.Returns(new CarEntity());
			var carId = new CarId(Rnd.Lng);
			var userId = new AuthUserId(Rnd.Lng);
			var query = new CheckCarBelongsToUserQuery(userId, carId);

			// Act
			await handler.HandleAsync(query, CancellationToken.None);

			// Assert
			var calls = fluent.ReceivedCalls();
			Assert.Collection(calls,
				c => Helpers.AssertWhere<CarEntity, CarId>(c, x => x.Id, Compare.Equal, carId),
				c => Helpers.AssertWhere<CarEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
				_ => { }
			);
		}

		public class Receives_None
		{
			[Fact]
			public async Task Audits_Msg()
			{
				// Arrange
				var (_, fluent, log, handler) = Setup();
				var msg = new TestMsg();
				fluent.QuerySingleAsync<CarEntity>()
					.Returns(F.None<CarEntity>(msg));
				var query = new CheckCarBelongsToUserQuery(new(), new());

				// Act
				await handler.HandleAsync(query, CancellationToken.None);

				// Assert
				log.Received().Msg(msg);
			}

			[Fact]
			public async Task Returns_False()
			{
				// Arrange
				var (_, fluent, _, handler) = Setup();
				fluent.QuerySingleAsync<CarEntity>()
					.Returns(Create.None<CarEntity>());
				var query = new CheckCarBelongsToUserQuery(new(), new());

				// Act
				var result = await handler.HandleAsync(query, CancellationToken.None);

				// Assert
				result.AssertFalse();
			}
		}

		public class Receives_Some
		{
			[Fact]
			public async Task Returns_True()
			{
				// Arrange
				var (_, fluent, _, handler) = Setup();
				var entity = new CarEntity();
				fluent.QuerySingleAsync<CarEntity>()
					.Returns(entity);
				var query = new CheckCarBelongsToUserQuery(new(), new());

				// Act
				var result = await handler.HandleAsync(query, CancellationToken.None);

				// Assert
				result.AssertTrue();
			}
		}
	}

	public sealed record class TestMsg : Msg;
}
