// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.CheckPlaceCanBeDeleted.CheckPlaceCanBeDeletedHandler_Tests;

public class CountJourneysWithAsync_Tests : Abstracts.CheckCanBeDeleted.CountJourneysWithAsync_Tests
{
	private class Setup : Setup<CheckPlaceCanBeDeletedQuery, CheckPlaceCanBeDeletedHandler, PlaceId>
	{
		internal override CheckPlaceCanBeDeletedHandler GetHandler(Vars v) =>
			new(v.Repo, Substitute.For<ISettingsRepository>(), v.Log);
	}

	[Fact]
	public override async Task Test00_Calls_FluentQuery_Where__With_Correct_Values()
	{
		await new Setup().Test00(
			h => h.CountJourneysWithAsync,
			(c, placeId) => Helpers.AssertWhere(
				c,
				"(journey_from_place_id = @placeId OR journey_to_place_ids ? @placeId::text)", new { placeId }

			)
		);
	}

	[Fact]
	public override async Task Test01_Calls_FluentQuery_CountAsync()
	{
		await new Setup().Test01(h => h.CountJourneysWithAsync);
	}

	[Fact]
	public override async Task Test02_Calls_FluentQuery_CountAsync__Returns_Result()
	{
		await new Setup().Test02(h => h.CountJourneysWithAsync);
	}
}
