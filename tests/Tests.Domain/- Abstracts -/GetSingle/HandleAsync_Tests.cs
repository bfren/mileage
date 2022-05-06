// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Data;
using Jeebs.Data.Enums;
using Jeebs.Data.Testing.Query;
using Jeebs.Messages;
using Mileage.Domain;
using StrongId;

namespace Abstracts.GetSingle;

public abstract class HandleAsync_Tests
{
	public abstract Task Test00_Id_Is_Null__Returns_None_With_NullMsg();

	public abstract Task Test01_Calls_Log_Vrb__With_Correct_Values();

	public abstract Task Test02_Calls_FluentQuery_Where__With_Correct_Values();

	public abstract Task Test03_Calls_FluentQuery_QuerySingleAsync();

	public abstract Task Test04_Calls_FluentQuery_QuerySingleAsync__Different_UserId__Returns_None_With_DoesNotBelongToUserMsg();

	public abstract Task Test05_Calls_FluentQuery_QuerySingleAsync__Same_UserId__Returns_Result();

	internal abstract class Setup<TRepo, TEntity, TId, TQuery, THandler, TModel> : TestHandler.Setup<TRepo, TEntity, TId, THandler>
		where TRepo : class, IRepository<TEntity, TId>
		where TEntity : IWithId<TId>
		where TId : LongId, new()
		where TQuery : IQuery<TModel>
		where THandler : QueryHandler<TQuery, TModel>
		where TModel : IWithUserId, IWithId<TId>
	{
		internal string Name { get; }

		internal bool EnableCache { get; }

		internal abstract TQuery GetQuery(AuthUserId? userId = null, TId? entityId = null);

		internal abstract TModel NewModel { get; }

		protected Setup(string name, bool enableCache) =>
			(Name, EnableCache) = (name, enableCache);

		internal override (THandler handler, Vars v) GetVars()
		{
			var (handler, v) = base.GetVars();
			if (EnableCache)
			{
				v.Cache.GetOrCreateAsync(Arg.Any<TId>(), Arg.Any<Func<Task<Maybe<TModel>>>>())
					.Returns(x => x.ArgAt<Func<Task<Maybe<TModel>>>>(1).Invoke());

				v.Fluent.QuerySingleAsync<TModel>()
					.Returns(NewModel);
			}

			return (handler, v);
		}

		internal async Task Test00<TIdIsNullMsg>(Func<THandler, TQuery, Task<Maybe<TModel>>> handle)
			where TIdIsNullMsg : IMsg
		{
			// Arrange
			var (handler, _) = GetVars();
			var query = GetQuery(LongId<AuthUserId>(), new() { Value = 0 });

			// Act
			var result = await handle(handler, query);

			// Assert
			result.AssertNone().AssertType<TIdIsNullMsg>();
		}

		internal async Task Test01(Func<THandler, TQuery, Task<Maybe<TModel>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var query = GetQuery();

			// Act
			_ = await handle(handler, query);

			// Assert
			v.Log.Received().Vrb($"Get {Name}: {{Query}}.", query);
		}

		internal async Task Test02(Func<THandler, TQuery, Task<Maybe<TModel>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var userId = LongId<AuthUserId>();
			var entityId = LongId<TId>();
			var query = GetQuery(userId, entityId);

			// Act
			_ = await handle(handler, query);

			// Assert
			v.Fluent.AssertCalls(
				c => FluentQueryHelper.AssertWhere<TEntity, TId>(c, x => x.Id, Compare.Equal, entityId),
				c => FluentQueryHelper.AssertWhere<TEntity, AuthUserId>(c, "UserId", Compare.Equal, userId),
				_ => { }
			);
		}

		internal async Task Test03(Func<THandler, TQuery, Task<Maybe<TModel>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var query = GetQuery();

			// Act
			_ = await handle(handler, query);

			// Assert
			await v.Fluent.Received().QuerySingleAsync<TModel>();
		}

		internal async Task Test04<TDoesNotBelongToUserMsg>(Func<THandler, TQuery, Task<Maybe<TModel>>> handle)
			where TDoesNotBelongToUserMsg : Msg
		{
			// Arrange
			var (handler, v) = GetVars();
			var model = NewModel;
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(model);
			var query = GetQuery();

			// Act
			var result = await handle(handler, query);

			// Assert
			result.AssertNone().AssertType<TDoesNotBelongToUserMsg>();
		}

		internal async Task Test05(Func<THandler, TQuery, Task<Maybe<TModel>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var model = NewModel;
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(model);
			var query = GetQuery(model.UserId, model.Id);

			// Act
			var result = await handle(handler, query);

			// Assert
			var some = result.AssertSome();
			Assert.Equal(model, some);
		}
	}
}
