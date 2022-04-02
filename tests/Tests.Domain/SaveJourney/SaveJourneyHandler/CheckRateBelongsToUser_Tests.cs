// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveJourney.SaveJourneyHandler_Tests;

public class CheckRateBelongsToUser_Tests : Abstracts.TestHandler
{
	private class Setup : Setup<IJourneyRepository, JourneyEntity, JourneyId, SaveJourneyHandler>
	{
		internal override SaveJourneyHandler GetHandler(Vars v) =>
			new(v.Dispatcher, v.Repo, v.Log);
	}

	private (SaveJourneyHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task RateId_Null__Returns_True()
	{
		// Arrange
		var (handler, v) = GetVars();

		// Act
		var result = await handler.CheckRateBelongsToUser(LongId<AuthUserId>(), null);

		// Assert
		Assert.True(result);
		await v.Dispatcher.DidNotReceiveWithAnyArgs().DispatchAsync(default!);
	}

	[Fact]
	public async Task Calls_Dispatcher_DispatchAsync__Receives_Some__Returns_Value()
	{
		// Arrange
		var (handler, v) = GetVars();
		var value = Rnd.Flip;
		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(F.Some(value).AsTask);

		// Act
		var result = await handler.CheckRateBelongsToUser(LongId<AuthUserId>(), LongId<RateId>());

		// Assert
		Assert.Equal(value, result);
	}

	[Fact]
	public async Task Calls_Dispatcher_DispatchAsync__Receives_Some_False__Logs_To_Dbg()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(false);
		var userId = LongId<AuthUserId>();
		var rateId = LongId<RateId>();

		// Act
		await handler.CheckRateBelongsToUser(userId, rateId);

		// Assert
		v.Log.Received().Dbg("Rate {RateId} does not belong to user {UserId}.", rateId.Value, userId.Value);
	}

	[Fact]
	public async Task Calls_Dispatcher_DispatchAsync__Receives_None__Returns_False()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(Create.None<bool>());

		// Act
		var result = await handler.CheckRateBelongsToUser(LongId<AuthUserId>(), LongId<RateId>());

		// Assert
		Assert.False(result);
	}
}
