// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Data.Enums;
using Jeebs.Data.Testing.Query;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.CheckPlacesBelongToUser.CheckPlacesBelongToUserHandler_Tests;

public sealed class HandleAsync_Tests : Abstracts.TestHandler
{
	private sealed class Setup : Setup<IPlaceRepository, PlaceEntity, PlaceId, CheckPlacesBelongToUserHandler>
	{
		internal override CheckPlacesBelongToUserHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);
	}

	private (CheckPlacesBelongToUserHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task No_PlaceIds__Returns_None_With_PlaceIdsIsNullMsg()
	{
		// Arrange
		var (handler, _) = GetVars();
		var query = new CheckPlacesBelongToUserQuery(IdGen.LongId<AuthUserId>(), []);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		result.AssertFailure();
	}

	[Fact]
	public async Task Calls_Log_Vrb__With_Query()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QueryAsync<PlaceEntity>()
			.Returns(Array.Empty<PlaceEntity>());
		var placeIds = new[] { IdGen.LongId<PlaceId>(), IdGen.LongId<PlaceId>() };
		var placeIdsValues = placeIds.Select(p => p.Value);
		var query = new CheckPlacesBelongToUserQuery(IdGen.LongId<AuthUserId>(), placeIds);

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Log.Received().Vrb(
			"Checking places {PlaceIds} belong to user {UserId}.",
			Arg.Is<IEnumerable<long>>(x => x.SequenceEqual(placeIdsValues)),
			query.UserId.Value
		);
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QueryAsync<PlaceEntity>()
			.Returns(Array.Empty<PlaceEntity>());
		var placeIds = new[] { IdGen.LongId<PlaceId>(), IdGen.LongId<PlaceId>() };
		var userId = IdGen.LongId<AuthUserId>();
		var query = new CheckPlacesBelongToUserQuery(userId, placeIds);

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Fluent.AssertCalls(
			c => FluentQueryHelper.AssertWhereIn<PlaceEntity, PlaceId>(c, x => x.Id, placeIds),
			c => FluentQueryHelper.AssertWhere<PlaceEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
			_ => { }
		);
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__Receives_None__Calls_Log_Msg()
	{
		// Arrange
		var (handler, v) = GetVars();
		var failure = FailGen.Create();
		v.Fluent.QueryAsync<PlaceEntity>()
			.Returns(failure);
		var query = new CheckPlacesBelongToUserQuery(new(), [IdGen.LongId<PlaceId>(), IdGen.LongId<PlaceId>()]);

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Log.Received().Failure(failure.Value);
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__Receives_None__Returns_False()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QueryAsync<PlaceEntity>()
				.Returns(FailGen.Create<IEnumerable<PlaceEntity>>());
		var query = new CheckPlacesBelongToUserQuery(new(), [IdGen.LongId<PlaceId>(), IdGen.LongId<PlaceId>()]);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		result.AssertFalse();
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__Receives_Some__Returns_True()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QueryAsync<PlaceEntity>()
			.Returns(new PlaceEntity[] { new(), new() });
		var query = new CheckPlacesBelongToUserQuery(new(), IdGen.LongId<PlaceId>(), IdGen.LongId<PlaceId>());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		result.AssertTrue();
	}
}
