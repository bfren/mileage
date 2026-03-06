// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Data.Enums;
using Jeebs.Data.Testing.Query;
using Mileage.Domain.CheckCarBelongsToUser;
using Mileage.Domain.CheckPlacesBelongToUser;
using Mileage.Domain.CheckRateBelongsToUser;
using Mileage.Domain.SaveJourney.Internals;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveJourney.SaveJourneyHandler_Tests;

public sealed class HandleAsync_Tests : Abstracts.TestHandler
{
	private sealed class Setup : Setup<IJourneyRepository, JourneyEntity, JourneyId, SaveJourneyHandler>
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
		var userId = IdGen.LongId<AuthUserId>();
		var carId = IdGen.LongId<CarId>();
		var query = new SaveJourneyQuery { UserId = userId, CarId = carId };

		v.Dispatcher.SendAsync(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<JourneyEntity>()
			.Returns(new JourneyEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().SendAsync(
			Arg.Is<CheckCarBelongsToUserQuery>(x => x.UserId == userId && x.CarId == carId)
		);
	}

	[Fact]
	public async Task Checks_FromPlace_Belongs_To_User_With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = IdGen.LongId<AuthUserId>();
		var fromPlaceId = IdGen.LongId<PlaceId>();
		var query = new SaveJourneyQuery { UserId = userId, FromPlaceId = fromPlaceId };

		v.Dispatcher.SendAsync(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<JourneyEntity>()
			.Returns(new JourneyEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().SendAsync(
			Arg.Is<CheckPlacesBelongToUserQuery>(x => x.UserId == userId && x.PlaceIds[0] == fromPlaceId)
		);
	}

	[Fact]
	public async Task Checks_ToPlaces_Belong_To_User_With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = IdGen.LongId<AuthUserId>();
		var toPlaceIds = new[] { IdGen.LongId<PlaceId>(), IdGen.LongId<PlaceId>() };
		var query = new SaveJourneyQuery { UserId = userId, ToPlaceIds = toPlaceIds };

		v.Dispatcher.SendAsync(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<JourneyEntity>()
			.Returns(new JourneyEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().SendAsync(
			Arg.Is<CheckPlacesBelongToUserQuery>(x => x.UserId == userId && x.PlaceIds == toPlaceIds)
		);
	}

	[Fact]
	public async Task Checks_Rate_Belongs_To_User_With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = IdGen.LongId<AuthUserId>();
		var rateId = IdGen.LongId<RateId>();
		var query = new SaveJourneyQuery { UserId = userId, RateId = rateId };

		v.Dispatcher.SendAsync(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<JourneyEntity>()
			.Returns(new JourneyEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().SendAsync(
			Arg.Is<CheckRateBelongsToUserQuery>(x => x.UserId == userId && x.RateId == rateId)
		);
	}

	[Fact]
	public async Task Car_Does_Not_Belong_To_User__Returns_None_With_SaveJourneyCheckFailedMsg()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new SaveJourneyQuery();

		v.Dispatcher.SendAsync(Arg.Any<CheckCarBelongsToUserQuery>())
			.ReturnsForAnyArgs(false);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		result.AssertFailure();
	}

	[Fact]
	public async Task Places_Do_Not_Belong_To_User__Returns_None_With_SaveJourneyCheckFailedMsg()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new SaveJourneyQuery();

		v.Dispatcher.SendAsync(Arg.Any<CheckPlacesBelongToUserQuery>())
			.ReturnsForAnyArgs(false);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		result.AssertFailure();
	}

	[Fact]
	public async Task Rate_Does_Not_Belong_To_User__Returns_None_With_SaveJourneyCheckFailedMsg()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new SaveJourneyQuery();

		v.Dispatcher.SendAsync(Arg.Any<CheckRateBelongsToUserQuery>())
			.ReturnsForAnyArgs(false);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		result.AssertFailure();
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_Where__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = IdGen.LongId<AuthUserId>();
		var journeyId = IdGen.LongId<JourneyId>();
		var query = new SaveJourneyQuery() { UserId = userId, JourneyId = journeyId };

		v.Dispatcher.SendAsync(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<JourneyEntity>()
			.Returns(new JourneyEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Fluent.AssertCalls(
			c => FluentQueryHelper.AssertWhere<JourneyEntity, JourneyId>(c, x => x.Id, Compare.Equal, journeyId),
			c => FluentQueryHelper.AssertWhere<JourneyEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
			_ => { }
		);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_Some__Dispatches_Update__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = IdGen.LongId<AuthUserId>();
		var journeyId = IdGen.LongId<JourneyId>();
		var version = Rnd.Lng;
		var day = Rnd.DateTime;
		var carId = IdGen.LongId<CarId>();
		var startMiles = Rnd.UInt32;
		var endMiles = startMiles + Rnd.UInt32;
		var fromPlaceId = IdGen.LongId<PlaceId>();
		var toPlaceIds = new[] { IdGen.LongId<PlaceId>(), IdGen.LongId<PlaceId>() };
		var rateId = IdGen.LongId<RateId>();
		var query = new SaveJourneyQuery(userId, journeyId, version, day, carId, startMiles, endMiles, fromPlaceId, toPlaceIds, rateId);

		v.Dispatcher.SendAsync(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<JourneyEntity>()
			.Returns(new JourneyEntity { Id = journeyId });

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().SendAsync(
			Arg.Is<UpdateJourneyCommand>(x =>
				x.JourneyId == journeyId
				&& x.Version == version
				&& x.Day == day
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
		var journeyId = IdGen.LongId<JourneyId>();
		var query = new SaveJourneyQuery();
		var updated = Rnd.Flip;

		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.SendAsync(Arg.Any<UpdateJourneyCommand>())
			.Returns(updated);
		v.Fluent.QuerySingleAsync<JourneyEntity>()
			.Returns(new JourneyEntity { Id = journeyId });

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().SendAsync(Arg.Any<UpdateJourneyCommand>());
		result.AssertOk(journeyId);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_None__Dispatches_Create__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = IdGen.LongId<AuthUserId>();
		var day = Rnd.DateTime;
		var carId = IdGen.LongId<CarId>();
		var startMiles = Rnd.UInt32;
		var endMiles = startMiles + Rnd.UInt32;
		var fromPlaceId = IdGen.LongId<PlaceId>();
		var toPlaceIds = new[] { IdGen.LongId<PlaceId>(), IdGen.LongId<PlaceId>() };
		var rateId = IdGen.LongId<RateId>();
		var query = new SaveJourneyQuery(userId, null, null, day, carId, startMiles, endMiles, fromPlaceId, toPlaceIds, rateId);

		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<JourneyEntity>()
			.Returns(FailGen.Create<JourneyEntity>());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().SendAsync(
			Arg.Is<CreateJourneyQuery>(x =>
				x.UserId == userId
				&& x.Day == day
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
		var journeyId = IdGen.LongId<JourneyId>();
		var query = new SaveJourneyQuery();

		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.SendAsync(Arg.Any<CreateJourneyQuery>())
			.Returns(journeyId);
		v.Fluent.QuerySingleAsync<JourneyEntity>()
			.Returns(FailGen.Create<JourneyEntity>());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().SendAsync(Arg.Any<CreateJourneyQuery>());
		result.AssertOk(journeyId);
	}
}
