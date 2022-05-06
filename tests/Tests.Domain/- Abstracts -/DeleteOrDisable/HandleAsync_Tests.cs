// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Data;
using Jeebs.Messages;
using Mileage.Domain;
using Mileage.Persistence.Common;
using StrongId;

namespace Abstracts.DeleteOrDisable;

public abstract class HandleAsync_Tests
{
	public abstract Task Test00_Calls_Log_Vrb__With_Query();

	public abstract Task Test01_Dispatches_Check_Item_Can_Be_Deleted_Query__With_Correct_Values();

	public abstract Task Test02_Calls_Delete_Or_Disable__Receives_Some_True__Removes_Item_From_Cache();

	public abstract Task Test03_Calls_Delete_Or_Disable__Receives_Some_False__Leaves_Cache_Alone();

	public abstract Task Test04_Calls_Delete_Or_Disable__Receives_None__Leaves_Cache_Alone();

	public abstract Task Test05_Calls_Delete_Or_Disable__Returns_Result();

	internal abstract class Setup<TRepo, TEntity, TId, TCommand, THandler, TModel, TCheckQuery> : TestHandler.Setup<TRepo, TEntity, TId, THandler>
		where TRepo : class, IRepository<TEntity, TId>
		where TEntity : IWithId<TId>
		where TId : LongId, new()
		where TCommand : ICommand
		where THandler : CommandHandler<TCommand>
		where TModel : IWithId<TId>
		where TCheckQuery : IWithUserId, IQuery<DeleteOperation>, IWithId<TId>
	{
		internal string Name { get; }

		internal abstract TCommand GetCommand(AuthUserId? userId = null, TId? entityId = null);

		internal abstract TModel EmptyModel { get; }

		protected Setup(string name) =>
			Name = name;

		internal (THandler handler, DeleteOrDisable<TId> dOrD, Vars v) GetVars(bool dOrDResult = true)
		{
			var (handler, v) = base.GetVars();
			v.Dispatcher.DispatchAsync(Arg.Any<TCheckQuery>())
				.Returns(F.Some(DeleteOperation.None));
			var dOrD = Substitute.For<DeleteOrDisable<TId>>();
			dOrD.Invoke(default!, default!, default)
				.ReturnsForAnyArgs(dOrDResult);

			return (handler, dOrD, v);
		}

		internal async Task Test00(Func<THandler, TCommand, DeleteOrDisable<TId>, Task<Maybe<bool>>> handle)
		{
			// Arrange
			var (handler, dOrD, v) = GetVars();
			var command = GetCommand();

			// Act
			_ = await handle(handler, command, dOrD);

			// Assert
			v.Log.Received().Vrb($"Delete or Disable {Name}: {{Command}}", command);
		}

		internal async Task Test01(Func<THandler, TCommand, DeleteOrDisable<TId>, Task<Maybe<bool>>> handle)
		{
			// Arrange
			var (handler, dOrD, v) = GetVars();
			var userId = LongId<AuthUserId>();
			var entityId = LongId<TId>();
			var command = GetCommand(userId, entityId);

			// Act
			_ = await handle(handler, command, dOrD);

			// Assert
			await v.Dispatcher.Received().DispatchAsync(Arg.Is<TCheckQuery>(x =>
				x.UserId == userId
				&& x.Id == entityId
			));
		}

		internal async Task Test02(Func<THandler, TCommand, DeleteOrDisable<TId>, Task<Maybe<bool>>> handle)
		{
			// Arrange
			var (handler, dOrD, v) = GetVars(dOrDResult: true);
			var userId = LongId<AuthUserId>();
			var entityId = LongId<TId>();
			var command = GetCommand(userId, entityId);

			// Act
			_ = await handle(handler, command, dOrD);

			// Assert
			v.Cache.Received().RemoveValue(entityId);
		}

		internal async Task Test03(Func<THandler, TCommand, DeleteOrDisable<TId>, Task<Maybe<bool>>> handle)
		{
			// Arrange
			var (handler, dOrD, v) = GetVars(dOrDResult: false);
			var command = GetCommand();

			// Act
			_ = await handle(handler, command, dOrD);

			// Assert
			v.Cache.DidNotReceiveWithAnyArgs().RemoveValue(default!);
		}

		internal async Task Test04(Func<THandler, TCommand, DeleteOrDisable<TId>, Task<Maybe<bool>>> handle)
		{
			// Arrange
			var (handler, dOrD, v) = GetVars();
			dOrD.Invoke(default!, default!, default)
				.ReturnsForAnyArgs(Create.None<bool>());
			var command = GetCommand();

			// Act
			_ = await handle(handler, command, dOrD);

			// Assert
			v.Cache.DidNotReceiveWithAnyArgs().RemoveValue(default!);
		}

		internal async Task Test05(Func<THandler, TCommand, DeleteOrDisable<TId>, Task<Maybe<bool>>> handle)
		{
			// Arrange
			var value = Rnd.Flip;
			var (handler, dOrD, v) = GetVars(dOrDResult: value);
			var command = GetCommand();

			// Act
			var result = await handle(handler, command, dOrD);

			// Assert
			var some = result.AssertSome();
			Assert.Equal(value, some);
		}

		public sealed record class TestMsg : Msg;
	}
}
