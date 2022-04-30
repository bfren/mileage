// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.CheckPlaceCanBeDeleted.CheckPlaceCanBeDeletedHandler_Tests;

public class CheckIsDefaultAsync_Tests : Abstracts.CheckCanBeDeleted.CheckIsDefaultAsync_Tests
{
	private class Setup : Setup<CheckPlaceCanBeDeletedQuery, CheckPlaceCanBeDeletedHandler, PlaceId>
	{
		internal override CheckPlaceCanBeDeletedHandler GetHandler(Vars v) =>
			new(Substitute.For<IJourneyRepository>(), v.Repo, v.Log);
	}

	[Fact]
	public override async Task Test00_Calls_FluentQuery_Where__With_Correct_Values()
	{
		await new Setup().Test00(h => h.CheckIsDefaultAsync);
	}

	[Fact]
	public override async Task Test01_Calls_FluentQuery_ExecuteAsync__With_Correct_Values()
	{
		await new Setup().Test01(h => h.CheckIsDefaultAsync, x => x.DefaultFromPlaceId);
	}

	[Fact]
	public override async Task Test02_Calls_FluentQuery_ExecuteAsync__Receives_Some__Returns_True_When_Ids_Match()
	{
		await new Setup().Test02(h => h.CheckIsDefaultAsync);
	}

	[Fact]
	public override async Task Test03_Calls_FluentQuery_ExecuteAsync__Receives_Some__Returns_False_When_Ids_Do_Not_Match()
	{
		await new Setup().Test03(h => h.CheckIsDefaultAsync);
	}

	[Fact]
	public override async Task Test04_Calls_FluentQuery_ExecuteAsync__Receives_None__Returns_None()
	{
		await new Setup().Test04(h => h.CheckIsDefaultAsync);
	}
}
