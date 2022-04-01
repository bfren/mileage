// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Mileage.Domain.CheckCarBelongsToUser;
using Mileage.Domain.CheckPlacesBelongToUser;
using Mileage.Domain.CheckRateBelongsToUser;
using Mileage.Domain.SaveJourney.Internals;
using Mileage.Domain.SaveJourney.Messages;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveJourney.SaveJourneyHandler_Tests;

public class HandleAsync_Tests : TestHandler
{
	private class Setup : Setup<IJourneyRepository, JourneyEntity, JourneyId, SaveJourneyHandler>
	{
		internal override SaveJourneyHandler GetHandler(Vars v) =>
			new(v.Dispatcher, v.Repo, v.Log);
	}

	private (SaveJourneyHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task Checks_Car_Belongs_To_User_With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var carId = LongId<CarId>();
		var query = new SaveJourneyQuery { UserId = userId, CarId = carId };

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<JourneyEntity>()
			.Returns(new JourneyEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<CheckCarBelongsToUserQuery>(q => q.UserId == userId && q.CarId == carId)
		);
	}

	[Fact]
	public async Task Checks_FromPlace_Belongs_To_User_With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var fromPlaceId = LongId<PlaceId>();
		var query = new SaveJourneyQuery { UserId = userId, FromPlaceId = fromPlaceId };

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<JourneyEntity>()
			.Returns(new JourneyEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<CheckPlacesBelongToUserQuery>(q => q.UserId == userId && q.PlaceIds[0] == fromPlaceId)
		);
	}

	[Fact]
	public async Task Checks_ToPlaces_Belong_To_User_With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var toPlaceIds = new[] { LongId<PlaceId>(), LongId<PlaceId>() };
		var query = new SaveJourneyQuery { UserId = userId, ToPlaceIds = toPlaceIds };

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<JourneyEntity>()
			.Returns(new JourneyEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<CheckPlacesBelongToUserQuery>(q => q.UserId == userId && q.PlaceIds == toPlaceIds)
		);
	}

	[Fact]
	public async Task Checks_Rate_Belongs_To_User_With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var rateId = LongId<RateId>();
		var query = new SaveJourneyQuery { UserId = userId, RateId = rateId };

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<JourneyEntity>()
			.Returns(new JourneyEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<CheckRateBelongsToUserQuery>(q => q.UserId == userId && q.RateId == rateId)
		);
	}

	[Fact]
	public async Task Car_Does_Not_Belong_To_User__Returns_None_With_SaveJourneyCheckFailedMsg()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new SaveJourneyQuery();

		v.Dispatcher.DispatchAsync(Arg.Any<CheckCarBelongsToUserQuery>())
			.ReturnsForAnyArgs(false);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var msg = result.AssertNone();
		Assert.IsType<SaveJourneyCheckFailedMsg>(msg);
	}

	[Fact]
	public async Task Places_Do_Not_Belong_To_User__Returns_None_With_SaveJourneyCheckFailedMsg()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new SaveJourneyQuery();

		v.Dispatcher.DispatchAsync(Arg.Any<CheckPlacesBelongToUserQuery>())
			.ReturnsForAnyArgs(false);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var msg = result.AssertNone();
		Assert.IsType<SaveJourneyCheckFailedMsg>(msg);
	}

	[Fact]
	public async Task Rate_Does_Not_Belong_To_User__Returns_None_With_SaveJourneyCheckFailedMsg()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new SaveJourneyQuery();

		v.Dispatcher.DispatchAsync(Arg.Any<CheckRateBelongsToUserQuery>())
			.ReturnsForAnyArgs(false);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var msg = result.AssertNone();
		Assert.IsType<SaveJourneyCheckFailedMsg>(msg);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_Where__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var journeyId = LongId<JourneyId>();
		var query = new SaveJourneyQuery() { UserId = userId, JourneyId = journeyId };

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<JourneyEntity>()
			.Returns(new JourneyEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		var calls = v.Fluent.ReceivedCalls();
		Assert.Collection(calls,
			c => Helpers.AssertWhere<JourneyEntity, JourneyId>(c, x => x.Id, Compare.Equal, journeyId),
			c => Helpers.AssertWhere<JourneyEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
			_ => { }
		);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_Some__Dispatches_Update__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var journeyId = LongId<JourneyId>();
		var version = Rnd.Lng;
		var date = Rnd.DateF.Get();
		var carId = LongId<CarId>();
		var startMiles = Rnd.UInt;
		var endMiles = startMiles + Rnd.UInt;
		var fromPlaceId = LongId<PlaceId>();
		var toPlaceIds = new[] { LongId<PlaceId>(), LongId<PlaceId>() };
		var rateId = LongId<RateId>();
		var query = new SaveJourneyQuery(userId, journeyId, version, date, carId, startMiles, endMiles, fromPlaceId, toPlaceIds, rateId);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<JourneyEntity>()
			.Returns(new JourneyEntity { Id = journeyId });

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<UpdateJourneyCommand>(x =>
				x.UserId == userId
				&& x.JourneyId == journeyId
				&& x.Version == version
				&& x.Date == date
				&& x.CarId == carId
				&& x.StartMiles == startMiles
				&& x.EndMiles == endMiles
				&& x.FromPlaceId == fromPlaceId
				&& x.ToPlaceIds == toPlaceIds
				&& x.RateId == rateId
			)
		);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_Some__Dispatches_Update__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var journeyId = LongId<JourneyId>();
		var query = new SaveJourneyQuery();
		var updated = Rnd.Flip;

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.DispatchAsync(Arg.Any<UpdateJourneyCommand>())
			.Returns(updated);
		v.Fluent.QuerySingleAsync<JourneyEntity>()
			.Returns(new JourneyEntity { Id = journeyId });

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(Arg.Any<UpdateJourneyCommand>());
		var some = result.AssertSome();
		Assert.Equal(journeyId, some);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_None__Dispatches_Create__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var date = Rnd.DateF.Get();
		var carId = LongId<CarId>();
		var startMiles = Rnd.UInt;
		var endMiles = startMiles + Rnd.UInt;
		var fromPlaceId = LongId<PlaceId>();
		var toPlaceIds = new[] { LongId<PlaceId>(), LongId<PlaceId>() };
		var rateId = LongId<RateId>();
		var query = new SaveJourneyQuery(userId, null, null, date, carId, startMiles, endMiles, fromPlaceId, toPlaceIds, rateId);

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<JourneyEntity>()
			.Returns(Create.None<JourneyEntity>());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(
			Arg.Is<CreateJourneyQuery>(x =>
				x.UserId == userId
				&& x.Date == date
				&& x.CarId == carId
				&& x.StartMiles == startMiles
				&& x.EndMiles == endMiles
				&& x.FromPlaceId == fromPlaceId
				&& x.ToPlaceIds == toPlaceIds
				&& x.RateId == rateId
			)
		);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_None__Dispatches_Create__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var journeyId = LongId<JourneyId>();
		var query = new SaveJourneyQuery();
		var updated = Rnd.Flip;

		v.Dispatcher.DispatchAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.DispatchAsync(Arg.Any<CreateJourneyQuery>())
			.Returns(journeyId);
		v.Fluent.QuerySingleAsync<JourneyEntity>()
			.Returns(Create.None<JourneyEntity>());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().DispatchAsync(Arg.Any<CreateJourneyQuery>());
		var some = result.AssertSome();
		Assert.Equal(journeyId, some);
	}
}
