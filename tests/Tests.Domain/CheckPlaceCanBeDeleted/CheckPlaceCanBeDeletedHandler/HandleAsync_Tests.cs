// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Domain.CheckPlaceCanBeDeleted.Messages;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.CheckPlaceCanBeDeleted.CheckPlaceCanBeDeletedHandler_Tests;

public class HandleAsync_Tests : Abstracts.CheckCanBeDeleted.HandleAsync_Tests
{
	private class Setup : Setup<PlaceId, CheckPlaceCanBeDeletedQuery, CheckPlaceCanBeDeletedHandler>
	{
		public Setup() : base("Place") { }

		internal override CheckPlaceCanBeDeletedHandler GetHandler(Vars v) =>
			new(Substitute.For<IJourneyRepository>(), Substitute.For<ISettingsRepository>(), v.Log);

		internal override CheckPlaceCanBeDeletedQuery GetQuery(AuthUserId? userId = null, PlaceId? entityId = null)
		{
			if (userId is not null && entityId is not null)
			{
				return new(userId, entityId);
			}

			return new(LongId<AuthUserId>(), LongId<PlaceId>());
		}
	}

	[Fact]
	public override async Task Test00_Calls_Log_Vrb__With_Correct_Values()
	{
		await new Setup().Test00(h => h.HandleAsync);
	}

	[Fact]
	public override async Task Test01_Checks_Is_Default__Receives_Some_True__Returns_None_With_IsDefaultMsg()
	{
		await new Setup().Test01<PlaceIsDefaultFromPlaceMsg>(h => h.HandleAsync);
	}

	[Fact]
	public override async Task Test02_Checks_Is_Default__Receives_None__Returns_None()
	{
		await new Setup().Test02(h => h.HandleAsync);
	}

	[Fact]
	public override async Task Test03_Counts_Journeys_With__Receives_MoreThan_Zero__Returns_Disable()
	{
		await new Setup().Test03(h => h.HandleAsync);
	}

	[Fact]
	public override async Task Test04_Counts_Journeys_With__Receives_Zero__Returns_Delete()
	{
		await new Setup().Test04(h => h.HandleAsync);
	}

	[Fact]
	public override async Task Test05_Counts_Journeys_With__Receives_Negative__Returns_None()
	{
		await new Setup().Test05(h => h.HandleAsync);
	}
}
