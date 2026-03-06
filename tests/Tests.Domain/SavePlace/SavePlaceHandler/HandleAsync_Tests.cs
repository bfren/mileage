// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Data.Enums;
using Jeebs.Data.Testing.Query;
using Mileage.Domain.CheckPlacesBelongToUser;
using Mileage.Domain.SavePlace.Internals;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SavePlace.SavePlaceHandler_Tests;

public sealed class HandleAsync_Tests : Abstracts.TestHandler
{
	private sealed class Setup : Setup<IPlaceRepository, PlaceEntity, PlaceId, SavePlaceHandler>
	{
		internal override SavePlaceHandler GetHandler(Vars v) =>
			new(v.Dispatcher, v.Repo, v.Log);
	}

	private (SavePlaceHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task Calls_Log_Vrb__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new SavePlaceQuery();
		v.Dispatcher.SendAsync(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<PlaceEntity>()
			.Returns(new PlaceEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Log.Received().Vrb("Saving Place {Query}.", query);
	}

	[Fact]
	public async Task Checks_Place_Belongs_To_User_With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = IdGen.LongId<AuthUserId>();
		var placeId = IdGen.LongId<PlaceId>();
		var query = new SavePlaceQuery(userId, placeId, Rnd.Lng, Rnd.Str, Rnd.Str, Rnd.Flip);

		v.Dispatcher.SendAsync(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<PlaceEntity>()
			.Returns(new PlaceEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().SendAsync(
			Arg.Is<CheckPlacesBelongToUserQuery>(x => x.UserId == userId && x.PlaceIds.Contains(placeId))
		);
	}

	[Fact]
	public async Task Place_Does_Not_Belong_To_User__Returns_None_With_PlaceDoesNotBelongToUserMsg()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new SavePlaceQuery(IdGen.LongId<AuthUserId>(), IdGen.LongId<PlaceId>(), Rnd.Lng, Rnd.Str, Rnd.Str, Rnd.Flip);

		v.Dispatcher.SendAsync(Arg.Any<CheckPlacesBelongToUserQuery>())
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
		var placeId = IdGen.LongId<PlaceId>();
		var query = new SavePlaceQuery(userId, placeId, Rnd.Lng, Rnd.Str, Rnd.Str, Rnd.Flip);

		v.Dispatcher.SendAsync(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<PlaceEntity>()
			.Returns(new PlaceEntity());

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Fluent.AssertCalls(
			c => FluentQueryHelper.AssertWhere<PlaceEntity, PlaceId>(c, x => x.Id, Compare.Equal, placeId),
			c => FluentQueryHelper.AssertWhere<PlaceEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
			_ => { }
		);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_Some__Dispatches_Update__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = IdGen.LongId<AuthUserId>();
		var placeId = IdGen.LongId<PlaceId>();
		var version = Rnd.Lng;
		var description = Rnd.Str;
		var postcode = Rnd.Str;
		var disabled = Rnd.Flip;
		var query = new SavePlaceQuery(userId, placeId, version, description, postcode, disabled);

		v.Dispatcher.SendAsync(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<PlaceEntity>()
			.Returns(new PlaceEntity { Id = placeId });

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().SendAsync(
			Arg.Is<UpdatePlaceCommand>(c =>
				c.Id == placeId
				&& c.Version == version
				&& c.Description == description
				&& c.Postcode == postcode
				&& c.IsDisabled == disabled
			)
		);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_Some__Dispatches_Update__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var placeId = IdGen.LongId<PlaceId>();
		var query = new SavePlaceQuery(IdGen.LongId<AuthUserId>(), IdGen.LongId<PlaceId>(), Rnd.Lng, Rnd.Str, Rnd.Str, Rnd.Flip);
		var updated = Rnd.Flip;

		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.SendAsync(Arg.Any<UpdatePlaceCommand>())
			.Returns(updated);
		v.Fluent.QuerySingleAsync<PlaceEntity>()
			.Returns(new PlaceEntity { Id = placeId });

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().SendAsync(Arg.Any<UpdatePlaceCommand>());
		result.AssertOk(placeId);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_None__Dispatches_Create__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = IdGen.LongId<AuthUserId>();
		var description = Rnd.Str;
		var postcode = Rnd.Str;
		var query = new SavePlaceQuery(userId, null, 0L, description, postcode, Rnd.Flip);

		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Fluent.QuerySingleAsync<PlaceEntity>()
			.Returns(FailGen.Create<PlaceEntity>());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().SendAsync(
			Arg.Is<CreatePlaceQuery>(c =>
				c.UserId == userId
				&& c.Description == description
				&& c.Postcode == postcode
			)
		);
	}

	[Fact]
	public async Task Checks_Pass__Calls_FluentQuery_QuerySingleAsync__Receives_None__Dispatches_Create__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var placeId = IdGen.LongId<PlaceId>();
		var query = new SavePlaceQuery(IdGen.LongId<AuthUserId>(), IdGen.LongId<PlaceId>(), Rnd.Lng, Rnd.Str, Rnd.Str, Rnd.Flip);

		v.Dispatcher.SendAsync<bool>(default!)
			.ReturnsForAnyArgs(true);
		v.Dispatcher.SendAsync(Arg.Any<CreatePlaceQuery>())
			.Returns(placeId);
		v.Fluent.QuerySingleAsync<PlaceEntity>()
			.Returns(FailGen.Create<PlaceEntity>());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		await v.Dispatcher.Received().SendAsync(Arg.Any<CreatePlaceQuery>());
		result.AssertOk(placeId);
	}
}
