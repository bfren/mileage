// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Data.Enums;
using Jeebs.Data.Testing.Query;
using Mileage.Domain.SaveSettings;
using Mileage.Persistence.Common;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.LoadSettings.LoadSettingsHandler_Tests;

public sealed class HandleAsync_Tests : Abstracts.TestHandler
{
	private sealed class Setup : Setup<ISettingsRepository, SettingsEntity, SettingsId, LoadSettingsHandler>
	{
		internal override LoadSettingsHandler GetHandler(Vars v) =>
			new(v.Repo, v.Dispatcher, v.Log);
	}

	private (LoadSettingsHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task Calls_Log_Vrb__With_Query()
	{
		// Arrange
		var (handler, v) = GetVars();
		v.Fluent.QuerySingleAsync<Settings>()
			.Returns(new Settings(IdGen.LongId<SettingsId>(), Rnd.Lng, IdGen.LongId<CarId>(), IdGen.LongId<PlaceId>(), IdGen.LongId<RateId>()));
		var query = new LoadSettingsQuery(IdGen.LongId<AuthUserId>());

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
			.Returns(new Settings(IdGen.LongId<SettingsId>(), Rnd.Lng, IdGen.LongId<CarId>(), IdGen.LongId<PlaceId>(), IdGen.LongId<RateId>()));
		var userId = IdGen.LongId<AuthUserId>();
		var query = new LoadSettingsQuery(userId);

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Fluent.AssertCalls(
			c => FluentQueryHelper.AssertWhere<SettingsEntity, AuthUserId>(c, x => x.UserId, Compare.Equal, userId),
			_ => { }
		);
	}

	[Fact]
	public async Task Calls_FluentQuery_QuerySingleAsync__Receives_Some__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var model = new Settings(IdGen.LongId<SettingsId>(), Rnd.Lng, IdGen.LongId<CarId>(), IdGen.LongId<PlaceId>(), IdGen.LongId<RateId>());
		v.Fluent.QuerySingleAsync<Settings>()
			.Returns(model);
		var query = new LoadSettingsQuery(IdGen.LongId<AuthUserId>());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		result.AssertOk(model);
	}
	[Fact]
	public async Task Calls_FluentQuery_QuerySingleAsync__Receives_None__Returns_Default_Settings()
	{
		// Arrange
		var (handler, v) = GetVars();
		var model = new Settings(IdGen.LongId<SettingsId>(), Rnd.Lng, IdGen.LongId<CarId>(), IdGen.LongId<PlaceId>(), IdGen.LongId<RateId>());
		v.Fluent.QuerySingleAsync<Settings>()
			.Returns(FailGen.Create<Settings>(), R.Wrap(model));
		v.Dispatcher.SendAsync(Arg.Any<SaveSettingsCommand>())
			.Returns(R.True);
		var query = new LoadSettingsQuery(IdGen.LongId<AuthUserId>());

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		result.AssertOk(model);
	}
}
