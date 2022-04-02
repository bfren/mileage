// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Data;
using Jeebs.Data.Enums;
using Jeebs.Messages;
using Mileage.Domain;
using StrongId;

namespace Abstracts.Delete;

public abstract class HandleAsync_Tests
{
	public abstract Task Test00_Calls_Log_Vrb__With_Query();

	public abstract Task Test01_Calls_FluentQuery_Where__With_Correct_Values();

	public abstract Task Test02_Calls_FluentQuery_WhereSingleAsync__Receives_None__Audits_Msg();

	public abstract Task Test03_Calls_FluentQuery_WhereSingleAsync__Receives_None__Returns_None_With_DoesNotExistMsg();

	public abstract Task Test04_Calls_FluentQuery_WhereSingleAsync__Receives_Some__Calls_Repo_DeleteAsync__With_Correct_Value();

	public abstract Task Test05_Calls_FluentQuery_WhereSingleAsync__Receives_Some__Calls_Repo_DeleteAsync__Returns_Result();

	internal abstract class Delete_Setup<TRepo, TEntity, TId, TCommand, THandler, TModel> : TestHandler.Setup<TRepo, TEntity, TId, THandler>
		where TRepo : class, IRepository<TEntity, TId>
		where TEntity : IWithId<TId>
		where TId : LongId, new()
		where TCommand : ICommand
		where THandler : CommandHandler<TCommand>
		where TModel : IWithId<TId>
	{
		internal string Name { get; }

		internal abstract TCommand GetCommand(AuthUserId? userId = null, TId? entityId = null);

		internal abstract TModel EmptyModel { get; }

		protected Delete_Setup(string name) =>
			Name = name;

		internal async Task Test00()
		{
			// Arrange
			var (handler, v) = GetVars();
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(EmptyModel);
			var command = GetCommand();

			// Act
			await handler.HandleAsync(command);

			// Assert
			v.Log.Received().Vrb($"Delete {Name}: {{Command}}", command);
		}

		internal async Task Test01()
		{
			// Arrange
			var (handler, v) = GetVars();
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(EmptyModel);
			var userId = LongId<AuthUserId>();
			var entityId = LongId<TId>();
			var command = GetCommand(userId, entityId);

			// Act
			await handler.HandleAsync(command);

			// Assert
			var calls = v.Fluent.ReceivedCalls();
			Assert.Collection(calls,
				c => Helpers.AssertWhere<TEntity, TId>(c, x => x.Id, Compare.Equal, entityId),
				c => Helpers.AssertWhere<TEntity, AuthUserId>(c, "UserId", Compare.Equal, userId),
				_ => { }
			);
		}

		internal async Task Test02()
		{
			// Arrange
			var (handler, v) = GetVars();
			var msg = new TestMsg();
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(F.None<TModel>(msg));
			var command = GetCommand();

			// Act
			await handler.HandleAsync(command);

			// Assert
			v.Log.Received().Msg(msg);
		}

		internal async Task Test03<TDoesNotExistMsg>(Func<TDoesNotExistMsg, AuthUserId> getUserId, Func<TDoesNotExistMsg, TId> getEntityId)
			where TDoesNotExistMsg : Msg
		{
			// Arrange
			var (handler, v) = GetVars();
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(Create.None<TModel>());
			var userId = LongId<AuthUserId>();
			var entityId = LongId<TId>();
			var command = GetCommand(userId, entityId);

			// Act
			var result = await handler.HandleAsync(command);

			// Assert
			var none = result.AssertNone();
			var msg = Assert.IsType<TDoesNotExistMsg>(none);
			Assert.Equal(userId, getUserId(msg));
			Assert.Equal(entityId, getEntityId(msg));
		}

		internal async Task Test04(Func<TId, long, TModel> getModel)
		{
			// Arrange
			var (handler, v) = GetVars();
			var userId = LongId<AuthUserId>();
			var carId = LongId<TId>();
			var query = GetCommand(userId, carId);
			var model = getModel(carId, Rnd.Lng);
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(model);

			// Act
			await handler.HandleAsync(query);

			// Assert
			await v.Repo.Received().DeleteAsync(model);
		}

		internal async Task Test05()
		{
			// Arrange
			var (handler, v) = GetVars();
			var model = EmptyModel;
			v.Fluent.QuerySingleAsync<TModel>()
				.Returns(model);
			var expected = Rnd.Flip;
			v.Repo.DeleteAsync<TModel>(default!)
				.ReturnsForAnyArgs(expected);
			var query = GetCommand();

			// Act
			var result = await handler.HandleAsync(query);

			// Assert
			var some = result.AssertSome();
			Assert.Equal(expected, some);
		}

		public sealed record class TestMsg : Msg;
	}
}
