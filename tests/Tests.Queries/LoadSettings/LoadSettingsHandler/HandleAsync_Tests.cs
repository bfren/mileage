// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Queries.LoadSettings.LoadSettingsHandler_Tests;

public class HandleAsync_Tests : TestHandler<ISettingsRepository, SettingsEntity, SettingsId, LoadSettingsHandler>
{
	public override LoadSettingsHandler GetHandler(Vars v) =>
		new(v.Repo, v.Log);

	[Fact]
	public async Task Calls_Log_Vrb__With_Query()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QuerySingleAsync<Settings>()
			.Returns(new Settings(new(Rnd.Lng), Rnd.Lng, new(Rnd.Lng), new(Rnd.Lng)));
		var query = new LoadSettingsQuery(new(Rnd.Lng));

		// Act
		await handler.HandleAsync(query, CancellationToken.None);

		// Assert
		v.Log.Received().Vrb("Load settings for User {UserId}", query.Id.Value);
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QuerySingleAsync<Settings>()
			.Returns(new Settings(new(Rnd.Lng), Rnd.Lng, new(Rnd.Lng), new(Rnd.Lng)));
		var userId = new AuthUserId(Rnd.Lng);
		var query = new LoadSettingsQuery(userId);

		// Act
		await handler.HandleAsync(query, CancellationToken.None);

		// Assert
		var calls = v.Fluent.ReceivedCalls();
		Assert.Collection(calls,
			c => Helpers.AssertWhere<SettingsEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
			_ => { }
		);
	}

	[Fact]
	public async Task Calls_FluentQuery_QuerySingleAsync__Receives_Some__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var model = new Settings(new(Rnd.Lng), Rnd.Lng, new(Rnd.Lng), new(Rnd.Lng));
		v.Fluent.QuerySingleAsync<Settings>()
			.Returns(model);
		var query = new LoadSettingsQuery(new(Rnd.Lng));

		// Act
		var result = await handler.HandleAsync(query, CancellationToken.None);

		// Assert
		var some = result.AssertSome();
		Assert.Same(model, some);
	}
	[Fact]
	public async Task Calls_FluentQuery_QuerySingleAsync__Receives_None__Returns_Default_Settings()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QuerySingleAsync<Settings>()
			.Returns(Create.None<Settings>());
		var query = new LoadSettingsQuery(new(Rnd.Lng));

		// Act
		var result = await handler.HandleAsync(query, CancellationToken.None);

		// Assert
		var some = result.AssertSome();
		Assert.Equal(new(), some);
	}
}
