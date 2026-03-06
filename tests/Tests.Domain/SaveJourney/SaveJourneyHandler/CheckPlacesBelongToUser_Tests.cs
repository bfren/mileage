// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveJourney.SaveJourneyHandler_Tests;

public class CheckPlacesBelongToUser_Tests : Abstracts.TestHandler
{
	private sealed class Setup : Setup<IJourneyRepository, JourneyEntity, JourneyId, SaveJourneyHandler>
	{
		internal override SaveJourneyHandler GetHandler(Vars v) =>
			new(v.Dispatcher, v.Repo, v.Log);
	}

	private (SaveJourneyHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task PlaceIds_Null__Returns_True()
	{
		// Arrange
		var (handler, v) = GetVars();

		// Act
		var result = await handler.CheckPlacesBelongToUser(IdGen.LongId<AuthUserId>(), null);

		// Assert
		Assert.True(result);
		await v.Dispatcher.DidNotReceiveWithAnyArgs().SendAsync(default!);
	}

	[Fact]
	public async Task Calls_Dispatcher_SendAsync__Receives_Some__Returns_Value()
	{
		// Arrange
		var (handler, v) = GetVars();
		var value = Rnd.Flip;
		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(R.Wrap(value).AsTask());

		// Act
		var result = await handler.CheckPlacesBelongToUser(IdGen.LongId<AuthUserId>(), IdGen.LongId<PlaceId>(), IdGen.LongId<PlaceId>());

		// Assert
		Assert.Equal(value, result);
	}

	[Fact]
	public async Task Calls_Dispatcher_SendAsync__Receives_Some_False__Logs_To_Dbg()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(false);
		var userId = IdGen.LongId<AuthUserId>();
		var placeIds = new[] { IdGen.LongId<PlaceId>(), IdGen.LongId<PlaceId>() };
		var placeIdValues = placeIds.Select(x => x.Value).ToArray();

		// Act
		await handler.CheckPlacesBelongToUser(userId, placeIds);

		// Assert
		v.Log.Received().Dbg(
			"Places {PlaceIds} do not all belong to user {UserId}.",
			Arg.Is<IEnumerable<long>>(x => x.SequenceEqual(placeIdValues)),
			userId.Value
		);
	}

	[Fact]
	public async Task Calls_Dispatcher_SendAsync__Receives_None__Returns_False()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(FailGen.Create<bool>());

		// Act
		var result = await handler.CheckPlacesBelongToUser(IdGen.LongId<AuthUserId>(), IdGen.LongId<PlaceId>(), IdGen.LongId<PlaceId>());

		// Assert
		Assert.False(result);
	}
}
