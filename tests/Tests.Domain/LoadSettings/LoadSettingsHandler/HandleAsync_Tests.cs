// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.LoadSettings.LoadSettingsHandler_Tests;

public class HandleAsync_Tests : Abstracts.TestHandler
{
	private class Setup : Setup<ISettingsRepository, SettingsEntity, SettingsId, LoadSettingsHandler>
	{
		internal override LoadSettingsHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);
	}

	private (LoadSettingsHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task Calls_Log_Vrb__With_Query()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QuerySingleAsync<Settings>()
			.Returns(new Settings(Rnd.Lng, LongId<CarId>(), LongId<PlaceId>(), LongId<RateId>()));
		var query = new LoadSettingsQuery(LongId<AuthUserId>());

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Log.Received().Vrb("Load settings for User {UserId}", query.Id.Value);
	}

	[Fact]
	public async Task Calls_FluentQuery_Where__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QuerySingleAsync<Settings>()
			.Returns(new Settings(Rnd.Lng, LongId<CarId>(), LongId<PlaceId>(), LongId<RateId>()));
		var userId = LongId<AuthUserId>();
		var query = new LoadSettingsQuery(userId);

		// Act
		await handler.HandleAsync(query);

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
		var model = new Settings(Rnd.Lng, LongId<CarId>(), LongId<PlaceId>(), LongId<RateId>());
		v.Fluent.QuerySingleAsync<Settings>()
			.Returns(model);
		var query = new LoadSettingsQuery(LongId<AuthUserId>());

		// Act
		var result = await handler.HandleAsync(query);

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
		var query = new LoadSettingsQuery(LongId<AuthUserId>());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var some = result.AssertSome();
		Assert.Equal(new(), some);
	}
}
