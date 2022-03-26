// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SavePlace.Internals.CreatePlace.CreatePlaceHandler_Tests;

public class HandleAsync_Tests : TestHandler<IPlaceRepository, PlaceEntity, PlaceId, CreatePlaceHandler>
{
	public override CreatePlaceHandler GetHandler(Vars v) =>
		new(v.Repo, v.Log);

	[Fact]
	public async Task Calls_Log_Vrb__With_Query()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new CreatePlaceQuery(new(), Rnd.Str, Rnd.Str);

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Log.Received().Vrb("Create Place: {Query}", query);
	}

	[Fact]
	public async Task Calls_Repo_CreateAsync__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = new AuthUserId(Rnd.Lng);
		var description = Rnd.Str;
		var postcode = Rnd.Str;
		var query = new CreatePlaceQuery(userId, description, postcode);

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Repo.Received().CreateAsync(Arg.Is<PlaceEntity>(p =>
			p.UserId == userId
			&& p.Description == description
			&& p.Postcode == postcode
		));
	}

	[Fact]
	public async void Calls_Repo_CreateAsync__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var expected = new PlaceId(Rnd.Lng);
		v.Repo.CreateAsync(default!)
			.ReturnsForAnyArgs(expected);
		var query = new CreatePlaceQuery(new(), Rnd.Str, Rnd.Str);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var some = result.AssertSome();
		Assert.Equal(expected, some);
	}
}
