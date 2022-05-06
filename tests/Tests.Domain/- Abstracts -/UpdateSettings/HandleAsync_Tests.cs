// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Domain;
using Mileage.Domain.SaveSettings.Messages;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;
using StrongId;

namespace Abstracts.UpdateSettings;

public abstract class HandleAsync_Tests
{
	public abstract Task Test00_ItemId_Is_Not_Null__Checks_Item_Belongs_To_User__Receives_None__Returns_None_With_SaveSettingsCheckFailedMsg();

	public abstract Task Test01_ItemId_Is_Not_Null__Checks_Item_Belongs_To_User__Receives_Some_False__Returns_None_With_SaveSettingsCheckFailedMsg();

	public abstract Task Test02_ItemId_Is_Not_Null__Checks_Item_Belongs_To_User__Receives_Some_True__Calls_Log_Vrb__With_Correct_Values();

	public abstract Task Test03_ItemId_Is_Not_Null__Checks_Item_Belongs_To_User__Receives_Some_True__Calls_Settings_UpdateAsync__With_Correct_Values();

	public abstract Task Test04_ItemId_Is_Null__Calls_Log_Vrb__With_Correct_Values();

	public abstract Task Test05_ItemId_Is_Null__Calls_Settings_UpdateAsync__With_Correct_Values();

	internal abstract class Setup<TCommand, THandler, TItemId> : TestHandler.Setup<ISettingsRepository, SettingsEntity, SettingsId, THandler>
		where TCommand : IWithUserId, ICommand, IWithId<SettingsId>
		where THandler : CommandHandler<TCommand>
		where TItemId : LongId, new()
	{
		internal string Name { get; }

		internal abstract TCommand GetCommand(AuthUserId? userId = null, TItemId? itemId = null);

		protected Setup(string name) =>
			Name = name;

		internal async Task Test00(Func<THandler, TCommand, Task<Maybe<bool>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var command = GetCommand(itemId: LongId<TItemId>());
			v.Dispatcher.DispatchAsync<bool>(query: default!)
				.ReturnsForAnyArgs(Create.None<bool>());

			// Act
			var result = await handle(handler, command);

			// Assert
			result.AssertNone().AssertType<SaveSettingsCheckFailedMsg>();
		}

		internal async Task Test01(Func<THandler, TCommand, Task<Maybe<bool>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var command = GetCommand(itemId: LongId<TItemId>());
			v.Dispatcher.DispatchAsync<bool>(query: default!)
				.ReturnsForAnyArgs(F.False);

			// Act
			var result = await handle(handler, command);

			// Assert
			result.AssertNone().AssertType<SaveSettingsCheckFailedMsg>();
		}

		internal async Task Test02(Func<THandler, TCommand, Task<Maybe<bool>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var userId = LongId<AuthUserId>();
			var command = GetCommand(userId, LongId<TItemId>());
			v.Dispatcher.DispatchAsync<bool>(query: default!)
				.ReturnsForAnyArgs(F.True);

			// Act
			_ = await handle(handler, command);

			// Assert
			v.Log.Received().Vrb($"Updating Default {Name} for {{User}}.", userId.Value);
		}

		internal async Task Test03(Func<THandler, TCommand, Task<Maybe<bool>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var command = GetCommand(itemId: LongId<TItemId>());
			v.Dispatcher.DispatchAsync<bool>(query: default!)
				.ReturnsForAnyArgs(F.True);

			// Act
			_ = await handle(handler, command);

			// Assert
			await v.Repo.Received().UpdateAsync(command);
		}

		internal async Task Test04(Func<THandler, TCommand, Task<Maybe<bool>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var userId = LongId<AuthUserId>();
			var command = GetCommand(userId);

			// Act
			_ = await handle(handler, command);

			// Assert
			v.Log.Received().Vrb($"Updating Default {Name} for {{User}}.", userId.Value);
		}

		internal async Task Test05(Func<THandler, TCommand, Task<Maybe<bool>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var command = GetCommand();

			// Act
			_ = await handle(handler, command);

			// Assert
			await v.Repo.Received().UpdateAsync(command);
		}
	}
}
