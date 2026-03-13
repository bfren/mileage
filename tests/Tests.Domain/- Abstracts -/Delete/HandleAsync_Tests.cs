// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Jeebs.Data.Common;
using Jeebs.Data.Enums;
using Jeebs.Data.Testing.Query;
using Wrap.Ids;

namespace Abstracts.Delete;

public abstract class HandleAsync_Tests
{
	public abstract Task Test00_Calls_Log_Vrb__With_Query();

	public abstract Task Test01_Calls_FluentQuery_Where__With_Correct_Values();

	public abstract Task Test02_Calls_FluentQuery_WhereSingleAsync__Receives_None__Audits_Msg();

	public abstract Task Test03_Calls_FluentQuery_WhereSingleAsync__Receives_None__Returns_None_With_DoesNotExistMsg();

	public abstract Task Test04_Calls_FluentQuery_WhereSingleAsync__Receives_Some__Calls_Repo_DeleteAsync__With_Correct_Value();

	public abstract Task Test05_Calls_FluentQuery_WhereSingleAsync__Receives_Some__Calls_Repo_DeleteAsync__Returns_Result();

	internal abstract class Setup<TRepo, TEntity, TId, TCommand, THandler, TModel> : TestHandler.Setup<TRepo, TEntity, TId, THandler>
		where TRepo : class, IRepository<TEntity, TId>
		where TEntity : IWithId<TId, long>
		where TId : LongId<TId>, new()
		where TCommand : Command
		where THandler : CommandHandler<TCommand>
		where TModel : IWithId<TId, long>
	{
		internal string Name { get; }

		internal abstract TCommand GetCommand(AuthUserId? userId = null, TId? entityId = null);

		internal abstract TModel EmptyModel { get; }

		protected Setup(string name) =>
			Name = name;

		internal async Task Test00(Func<THandler, TCommand, Task<Result<bool>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(EmptyModel);
			var command = GetCommand();

			// Act
			await handle(handler, command);

			// Assert
			v.Log.Received().Vrb($"Delete {Name}: {{Command}}", command);
		}

		internal async Task Test01(Func<THandler, TCommand, Task<Result<bool>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(EmptyModel);
			var userId = IdGen.LongId<AuthUserId>();
			var entityId = IdGen.LongId<TId>();
			var command = GetCommand(userId, entityId);

			// Act
			await handle(handler, command);

			// Assert
			v.Fluent.AssertCalls(
				c => FluentQueryHelper.AssertWhere<TEntity, TId>(c, x => x.Id, Compare.Equal, entityId),
				c => FluentQueryHelper.AssertWhere<TEntity, AuthUserId>(c, "UserId", Compare.Equal, userId),
				_ => { }
			);
		}

		internal async Task Test02(Func<THandler, TCommand, Task<Result<bool>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var failure = FailGen.Create();
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(failure);
			var command = GetCommand();

			// Act
			await handle(handler, command);

			// Assert
			v.Log.Received().Failure(failure.Value);
		}

		internal async Task Test03(
			Func<THandler, TCommand, Task<Result<bool>>> handle
		)
		{
			// Arrange
			var (handler, v) = GetVars();
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(FailGen.Create<TModel>());
			var userId = IdGen.LongId<AuthUserId>();
			var entityId = IdGen.LongId<TId>();
			var command = GetCommand(userId, entityId);

			// Act
			var result = await handle(handler, command);

			// Assert
			result.AssertFailure();
		}

		internal async Task Test04(Func<TId, long, TModel> getModel, Func<THandler, TCommand, Task<Result<bool>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var userId = IdGen.LongId<AuthUserId>();
			var entityId = IdGen.LongId<TId>();
			var command = GetCommand(userId, entityId);
			var model = getModel(entityId, Rnd.Lng);
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(model);

			// Act
			await handle(handler, command);

			// Assert
			await v.Repo.Received().DeleteAsync(model);
		}

		internal async Task Test05(Func<THandler, TCommand, Task<Result<bool>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var model = EmptyModel;
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(model);
			var expected = Rnd.Flip;
			v.Repo.DeleteAsync<TModel>(default!)
				.ReturnsForAnyArgs(expected);
			var command = GetCommand();

			// Act
			var result = await handle(handler, command);

			// Assert
			result.AssertOk(expected);
		}
	}
}
