// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Cqrs;
using Jeebs.Data.Common;
using Jeebs.Data.Enums;
using Jeebs.Data.Testing.Query;
using Mileage.Domain;
using Mileage.Persistence.Common;
using Wrap.Ids;

namespace Abstracts.DeleteOrDisable;

public abstract class DeleteOrDisableAsync_Tests
{
	public abstract Task Test00_Calls_FluentQuery_Where__With_Correct_Values();

	public abstract Task Test01_Calls_FluentQuery_WhereSingleAsync__Receives_None__Audits_Msg();

	public abstract Task Test02_Calls_FluentQuery_WhereSingleAsync__Receives_None__Returns_None_With_DoesNotExistMsg();

	public abstract Task Test03_Is_Delete__Calls_Repo_DeleteAsync__With_Correct_Values();

	public abstract Task Test04_Is_Disable__Calls_Repo_UpdateAsync__With_Correct_Values();

	public abstract Task Test05_Is_None__Returns_None_With_CannotBeDeletedMsg();

	internal abstract class Setup<TRepo, TEntity, TId, TCommand, THandler, TModel> : TestHandler.Setup<TRepo, TEntity, TId, THandler>
		where TRepo : class, IRepository<TEntity, TId>
		where TEntity : IWithId<TId, long>
		where TId : LongId<TId>, new()
		where TCommand : Command
		where THandler : CommandHandler<TCommand>
		where TModel : IWithId<TId, long>
	{
		public delegate DeleteOrDisable<TId> DeleteOrDisableAsyncMethod(THandler handler);

		internal abstract TModel EmptyModel { get; }

		protected Setup() { }

		internal async Task Test00(DeleteOrDisableAsyncMethod deleteOrDisable)
		{
			// Arrange
			var (handler, v) = GetVars();
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(EmptyModel);
			var userId = IdGen.LongId<AuthUserId>();
			var entityId = IdGen.LongId<TId>();
			var dOrD = deleteOrDisable(handler);

			// Act
			await dOrD(userId, entityId, DeleteOperation.None);

			// Assert
			v.Fluent.AssertCalls(
				c => FluentQueryHelper.AssertWhere<TEntity, TId>(c, x => x.Id, Compare.Equal, entityId),
				c => FluentQueryHelper.AssertWhere<TEntity, AuthUserId>(c, "UserId", Compare.Equal, userId),
				_ => { }
			);
		}

		internal async Task Test01(DeleteOrDisableAsyncMethod deleteOrDisable)
		{
			// Arrange
			var (handler, v) = GetVars();
			var failure = FailGen.Create();
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(failure);
			var userId = IdGen.LongId<AuthUserId>();
			var entityId = IdGen.LongId<TId>();
			var dOrD = deleteOrDisable(handler);

			// Act
			await dOrD(userId, entityId, DeleteOperation.None);

			// Assert
			v.Log.Received().Failure(failure.Value);
		}

		internal async Task Test02(DeleteOrDisableAsyncMethod deleteOrDisable)
		{
			// Arrange
			var (handler, v) = GetVars();
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(FailGen.Create<TModel>());
			var userId = IdGen.LongId<AuthUserId>();
			var entityId = IdGen.LongId<TId>();
			var dOrD = deleteOrDisable(handler);

			// Act
			var result = await dOrD(userId, entityId, DeleteOperation.None);

			// Assert
			result.AssertFailure();
		}

		internal async Task Test03(Func<TId, long, bool, TModel> getModel, DeleteOrDisableAsyncMethod deleteOrDisable)
		{
			// Arrange
			var (handler, v) = GetVars();
			var userId = IdGen.LongId<AuthUserId>();
			var entityId = IdGen.LongId<TId>();
			var model = getModel(entityId, Rnd.Lng, false);
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(model);
			var dOrD = deleteOrDisable(handler);

			// Act
			_ = await dOrD(userId, entityId, DeleteOperation.Delete);

			// Assert
			await v.Repo.Received().DeleteAsync(model);
		}

		internal async Task Test04(Func<TId, long, bool, TModel> getModel, DeleteOrDisableAsyncMethod deleteOrDisable)
		{
			// Arrange
			var (handler, v) = GetVars();
			var userId = IdGen.LongId<AuthUserId>();
			var entityId = IdGen.LongId<TId>();
			var model = getModel(entityId, Rnd.Lng, true);
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(model);
			var dOrD = deleteOrDisable(handler);

			// Act
			_ = await dOrD(userId, entityId, DeleteOperation.Disable);

			// Assert
			await v.Repo.Received().UpdateAsync(Arg.Is(model));
		}

		internal async Task Test05(DeleteOrDisableAsyncMethod deleteOrDisable)
		{
			// Arrange
			var (handler, v) = GetVars();
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(EmptyModel);
			var dOrD = deleteOrDisable(handler);

			// Act
			var result = await dOrD(IdGen.LongId<AuthUserId>(), IdGen.LongId<TId>(), DeleteOperation.None);

			// Assert
			result.AssertFailure();
		}
	}
}
