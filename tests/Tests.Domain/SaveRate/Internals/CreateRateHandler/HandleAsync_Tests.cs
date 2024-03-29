// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.SaveRate.Internals.CreateRate.CreateRateHandler_Tests;

public sealed class HandleAsync_Tests : Abstracts.TestHandler
{
	private sealed class Setup : Setup<IRateRepository, RateEntity, RateId, CreateRateHandler>
	{
		internal override CreateRateHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);
	}

	private (CreateRateHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	[Fact]
	public async Task Calls_Log_Vrb__With_Query()
	{
		// Arrange
		var (handler, v) = GetVars();
		var query = new CreateRateQuery(new(), (float)Rnd.Int / 100);

		// Act
		await handler.HandleAsync(query);

		// Assert
		v.Log.Received().Vrb("Create Rate: {Query}", query);
	}

	[Fact]
	public async Task Calls_Repo_CreateAsync__With_Correct_Values()
	{
		// Arrange
		var (handler, v) = GetVars();
		var userId = LongId<AuthUserId>();
		var amount = (float)Rnd.Int / 100;
		var query = new CreateRateQuery(userId, amount);

		// Act
		await handler.HandleAsync(query);

		// Assert
		await v.Repo.Received().CreateAsync(Arg.Is<RateEntity>(r =>
			r.UserId == userId
			&& r.AmountPerMileGBP == amount
		));
	}

	[Fact]
	public async void Calls_Repo_CreateAsync__Returns_Result()
	{
		// Arrange
		var (handler, v) = GetVars();
		var expected = LongId<RateId>();
		v.Repo.CreateAsync(default!)
			.ReturnsForAnyArgs(expected);
		var query = new CreateRateQuery(new(), (float)Rnd.Int / 100);

		// Act
		var result = await handler.HandleAsync(query);

		// Assert
		var some = result.AssertSome();
		Assert.Equal(expected, some);
	}
}
