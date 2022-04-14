// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;
using StrongId;

namespace Abstracts.UpdateJourney;

public abstract class HandleAsync_Tests
{
	public abstract Task Test00_Calls_Log_Vrb__With_Correct_Values();

	public abstract Task Test01_Calls_Repo_UpdateAsync__With_Correct_Values();

	public abstract Task Test02_Calls_Repo_Update_Async__Returns_Result();

	internal abstract class UpdateJourney_Setup<TCommand, THandler> : TestHandler.Setup<IJourneyRepository, JourneyEntity, JourneyId, THandler>
		where TCommand : ICommand, IWithId<JourneyId>
		where THandler : CommandHandler<TCommand>
	{
		internal string Name { get; }

		internal abstract TCommand GetCommand(AuthUserId? userId = null);

		protected UpdateJourney_Setup(string name) =>
			Name = name;

		internal async Task Test00(Func<THandler, TCommand, Task<Maybe<bool>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var command = GetCommand();

			// Act
			_ = await handle(handler, command);

			// Assert
			v.Log.Received().Vrb($"Updating {Name} for {{Journey}}.", command);
		}

		internal async Task Test01(Func<THandler, TCommand, Task<Maybe<bool>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var command = GetCommand();

			// Act
			_ = await handle(handler, command);

			// Assert
			await v.Repo.Received().UpdateAsync(command);
		}

		internal async Task Test02(Func<THandler, TCommand, Task<Maybe<bool>>> handle)
		{
			// Arrange
			var (handler, v) = GetVars();
			var command = GetCommand();
			var value = Rnd.Flip;
			v.Repo.UpdateAsync(command)
				.Returns(value);

			// Act
			var result = await handle(handler, command);

			// Assert
			Assert.Equal(value, result);
		}
	}
}
