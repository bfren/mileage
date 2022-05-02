// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Data.Testing.Query;
using Mileage.Domain;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;
using NSubstitute.Core;
using StrongId;

namespace Abstracts.CheckCanBeDeleted;

public abstract class CountJourneysWithAsync_Tests
{
	public abstract Task Test00_Calls_FluentQuery_Where__With_Correct_Values();

	public abstract Task Test01_Calls_FluentQuery_CountAsync();

	public abstract Task Test02_Calls_FluentQuery_CountAsync__Returns_Result();

	internal abstract class Setup<TQuery, THandler, TId> : TestHandler.Setup<IJourneyRepository, JourneyEntity, JourneyId, THandler>
		where TQuery : IQuery<DeleteOperation>
		where THandler : QueryHandler<TQuery, DeleteOperation>
		where TId : LongId, new()
	{
		public delegate CountJourneysWith<TId> CountJourneysWithAsyncMethod(THandler handler);

		protected Setup() { }

		internal async Task Test00(CountJourneysWithAsyncMethod countJourneysWith, Action<ICall, TId> whereId)
		{
			// Arrange
			var (handler, v) = GetVars();
			var userId = LongId<AuthUserId>();
			var entityId = LongId<TId>();
			var count = countJourneysWith(handler);

			// Act
			_ = await count(userId, entityId);

			// Assert
			v.Fluent.AssertCalls(
				c => FluentQueryHelper.AssertWhere<JourneyEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
				c => whereId(c, entityId),
				_ => { }
			);
		}

		internal async Task Test01(CountJourneysWithAsyncMethod countJourneysWith)
		{
			// Arrange
			var (handler, v) = GetVars();
			var count = countJourneysWith(handler);

			// Act
			_ = await count(LongId<AuthUserId>(), LongId<TId>());

			// Assert
			await v.Fluent.Received().CountAsync();
		}

		internal async Task Test02(CountJourneysWithAsyncMethod countJourneysWith)
		{
			// Arrange
			var (handler, v) = GetVars();
			var value = Rnd.Lng;
			v.Fluent.CountAsync()
				.Returns(value);
			var count = countJourneysWith(handler);

			// Act
			var result = await count(LongId<AuthUserId>(), LongId<TId>());

			// Assert
			var some = result.AssertSome();
			Assert.Equal(value, some);
		}
	}
}
