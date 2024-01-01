// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Linq.Expressions;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Data;
using Jeebs.Data.Enums;
using Jeebs.Data.Testing.Query;
using StrongId;

namespace Abstracts.GetEnumerable;

public abstract class HandleAsync_Tests
{
	public abstract Task Test00_Id_Is_Null__Returns_None_With_NullMsg();

	public abstract Task Test01_Calls_Log_Vrb__With_Correct_Values();

	public abstract Task Test02_Calls_FluentQuery_Where__With_Correct_Values();

	public abstract Task Test03_Calls_FluentQuery_Sort__With_Correct_Values();

	public abstract Task Test04_Calls_FluentQuery_QueryAsync();

	public abstract Task Test05_Calls_FluentQuery_QueryAsync__Returns_Result();

	internal abstract class Setup<TRepo, TEntity, TId, TQuery, THandler, TModel> : TestHandler.Setup<TRepo, TEntity, TId, THandler>
		where TRepo : class, IRepository<TEntity, TId>
		where TEntity : IWithId<TId>
		where TId : LongId, new()
		where TQuery : Query<IEnumerable<TModel>>
		where THandler : QueryHandler<TQuery, IEnumerable<TModel>>
	{
		internal string Name { get; }

		internal abstract (TQuery, AuthUserId) GetQuery(AuthUserId? userId = null);

		internal abstract TModel NewModel { get; }

		protected Setup(string name) =>
			Name = name;

		internal async Task Test00<TIdIsNullMsg>(Func<THandler, TQuery, Task<Maybe<IEnumerable<TModel>>>> handle)
			where TIdIsNullMsg : IMsg
		{
			// Arrange
			var (handler, _) = GetVars();
			var (query, _) = GetQuery(new() { Value = 0 });

			// Act
			var result = await handle(handler, query);

			// Assert
			result.AssertNone().AssertType<TIdIsNullMsg>();
		}

		internal async Task Test01(Func<THandler, TQuery, Task<Maybe<IEnumerable<TModel>>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var (query, userId) = GetQuery();

			// Act
			_ = await handle(handler, query);

			// Assert
			v.Log.Received().Vrb($"Get {Name} for {{User}}.", userId);
		}

		internal async Task Test02(Func<THandler, TQuery, Task<Maybe<IEnumerable<TModel>>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var userId = LongId<AuthUserId>();
			var (query, _) = GetQuery(userId);

			// Act
			_ = await handle(handler, query);

			// Assert
			v.Fluent.AssertCalls(
				c => FluentQueryHelper.AssertWhere<TEntity, AuthUserId>(c, "UserId", Compare.Equal, userId),
				_ => { },
				_ => { }
			);
		}

		internal async Task Test02_WithIsDisabled(Func<AuthUserId, bool, TQuery> getQuery, Func<THandler, TQuery, Task<Maybe<IEnumerable<TModel>>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var userId = LongId<AuthUserId>();
			var includeDisabled = Rnd.Flip;
			var disabledMatch = includeDisabled ? [true, false] : new[] { false };
			var query = getQuery(userId, includeDisabled);

			// Act
			_ = await handle(handler, query);

			// Assert
			v.Fluent.AssertCalls(
				c => FluentQueryHelper.AssertWhere<TEntity, AuthUserId>(c, "UserId", Compare.Equal, userId),
				c => FluentQueryHelper.AssertWhereIn<TEntity, bool>(c, "IsDisabled", disabledMatch),
				_ => { },
				_ => { }
			);
		}

		internal async Task Test03<TSort>(
			Expression<Func<TEntity, TSort>> sortBy,
			SortOrder sortOrder,
			Func<THandler, TQuery, Task<Maybe<IEnumerable<TModel>>>> handle
		)
		{
			// Arrange
			var (handler, v) = GetVars();
			var (query, _) = GetQuery();

			// Act
			_ = await handle(handler, query);

			// Assert
			v.Fluent.AssertCalls(
				_ => { },
				c => FluentQueryHelper.AssertSort(c, sortBy, sortOrder),
				_ => { }
			);
		}

		internal async Task Test03_WithIsDisabled<TSort>(
			Expression<Func<TEntity, TSort>> sortBy,
			SortOrder sortOrder,
			Func<THandler, TQuery, Task<Maybe<IEnumerable<TModel>>>> handle
		)
		{
			// Arrange
			var (handler, v) = GetVars();
			var (query, _) = GetQuery();

			// Act
			_ = await handle(handler, query);

			// Assert
			v.Fluent.AssertCalls(
				_ => { },
				_ => { },
				c => FluentQueryHelper.AssertSort(c, sortBy, sortOrder),
				_ => { }
			);
		}

		internal async Task Test04(Func<THandler, TQuery, Task<Maybe<IEnumerable<TModel>>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var (query, _) = GetQuery();

			// Act
			_ = await handle(handler, query);

			// Assert
			await v.Fluent.Received().QueryAsync<TModel>();
		}

		internal async Task Test05(Func<THandler, TQuery, Task<Maybe<IEnumerable<TModel>>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var m0 = NewModel;
			var m1 = NewModel;
			var m2 = NewModel;
			v.Fluent.QueryAsync<TModel>()
				.Returns(new[] { m0, m1, m2 });
			var (query, _) = GetQuery();

			// Act
			var result = await handle(handler, query);

			// Assert
			var some = result.AssertSome();
			Assert.Collection(some,
				x => Assert.Equal(m0, x),
				x => Assert.Equal(m1, x),
				x => Assert.Equal(m2, x)
			);
		}
	}
}
