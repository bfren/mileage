// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Linq.Expressions;
using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Jeebs.Data.Enums;
using Jeebs.Data.Testing.Query;
using Mileage.Domain;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;
using Wrap.Ids;

namespace Abstracts.CheckCanBeDeleted;

public abstract class CheckIsDefaultAsync_Tests
{
	public abstract Task Test00_Calls_FluentQuery_Where__With_Correct_Values();

	public abstract Task Test01_Calls_FluentQuery_ExecuteAsync__With_Correct_Values();

	public abstract Task Test02_Calls_FluentQuery_ExecuteAsync__Receives_Some__Returns_True_When_Ids_Match();

	public abstract Task Test03_Calls_FluentQuery_ExecuteAsync__Receives_Some__Returns_False_When_Ids_Do_Not_Match();

	public abstract Task Test04_Calls_FluentQuery_ExecuteAsync__Receives_None__Returns_None();

	internal abstract class Setup<TQuery, THandler, TId> : TestHandler.Setup<ISettingsRepository, SettingsEntity, SettingsId, THandler>
		where TQuery : Query<DeleteOperation>
		where THandler : QueryHandler<TQuery, DeleteOperation>
		where TId : LongId<TId>, new()
	{
		public delegate CheckIsDefault<TId> CheckIsDefaultAsyncMethod(THandler handler);

		protected Setup() { }

		internal async Task Test00(CheckIsDefaultAsyncMethod checkIsDefault)
		{
			// Arrange
			var (handler, v) = GetVars();
			var userId = IdGen.LongId<AuthUserId>();
			var check = checkIsDefault(handler);
			v.Fluent.ExecuteAsync<TId>(aliasSelector: default!)
				.ReturnsForAnyArgs(IdGen.LongId<TId>());

			// Act
			_ = await check(userId, IdGen.LongId<TId>());

			// Assert
			v.Fluent.AssertCalls(
				c => FluentQueryHelper.AssertWhere<SettingsEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
				_ => { }
			);
		}

		internal async Task Test01(CheckIsDefaultAsyncMethod checkIsDefault, Expression<Func<SettingsEntity, TId?>> property)
		{
			// Arrange
			var (handler, v) = GetVars();
			var check = checkIsDefault(handler);
			v.Fluent.ExecuteAsync<TId>(aliasSelector: default!)
				.ReturnsForAnyArgs(IdGen.LongId<TId>());

			// Act
			_ = await check(IdGen.LongId<AuthUserId>(), IdGen.LongId<TId>());

			// Assert
			v.Fluent.AssertCalls(
				_ => { },
				c => FluentQueryHelper.AssertExecute(c, property)
			);
		}

		internal async Task Test02(CheckIsDefaultAsyncMethod checkIsDefault)
		{
			// Arrange
			var (handler, v) = GetVars();
			var entityId = IdGen.LongId<TId>();
			v.Fluent.ExecuteAsync<TId?>(aliasSelector: default!)
				.ReturnsForAnyArgs(entityId);
			var check = checkIsDefault(handler);

			// Act
			var result = await check(IdGen.LongId<AuthUserId>(), entityId);

			// Assert
			var some = result.AssertOk();
			Assert.True(some);
		}

		internal async Task Test03(CheckIsDefaultAsyncMethod checkIsDefault)
		{
			// Arrange
			var (handler, v) = GetVars();
			var entityId = IdGen.LongId<TId>();
			v.Fluent.ExecuteAsync<TId?>(aliasSelector: default!)
				.ReturnsForAnyArgs(entityId);
			var check = checkIsDefault(handler);

			// Act
			var result = await check(IdGen.LongId<AuthUserId>(), IdGen.LongId<TId>());

			// Assert
			var some = result.AssertOk();
			Assert.False(some);
		}

		internal async Task Test04(CheckIsDefaultAsyncMethod checkIsDefault)
		{
			// Arrange
			var (handler, v) = GetVars();
			var failure = FailGen.Create<TId?>();
			v.Fluent.ExecuteAsync<TId?>(aliasSelector: default!)
				.ReturnsForAnyArgs(failure);
			var check = checkIsDefault(handler);

			// Act
			var result = await check(IdGen.LongId<AuthUserId>(), IdGen.LongId<TId>());

			// Assert
			result.AssertFailure();
		}
	}
}
