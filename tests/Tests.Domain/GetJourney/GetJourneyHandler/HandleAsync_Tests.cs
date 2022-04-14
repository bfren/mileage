// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Collections;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetJourney.GetJourney_Tests;

public class HandleAsync_Tests : Abstracts.GetSingle.HandleAsync_Tests
{
	private class Setup : GetSingle_Setup<IJourneyRepository, JourneyEntity, JourneyId, GetJourneyQuery, GetJourneyHandler, GetJourneyModel>
	{
		public Setup() : base("Journey") { }

		internal override GetJourneyHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);

		internal override GetJourneyQuery GetQuery(AuthUserId? userId = null, JourneyId? entityId = null)
		{
			if (userId is not null && entityId is not null)
			{
				return new(userId, entityId);
			}

			return new(LongId<AuthUserId>(), LongId<JourneyId>());
		}

		internal override GetJourneyModel NewModel { get; } = new(LongId<JourneyId>(), Rnd.Lng, Rnd.DateTime, LongId<CarId>(),
			Rnd.Int, Rnd.Int, LongId<PlaceId>(), ImmutableList.Create(LongId<PlaceId>(), LongId<PlaceId>()), LongId<RateId>());
	}

	[Fact]
	public override async Task Test00_Id_Is_Null__Returns_None_With_NullMsg()
	{
		await new Setup().Test00<Messages.JourneyIdIsNullMsg>((h, q) => h.HandleAsync(q));
	}

	[Fact]
	public override async Task Test01_Calls_Log_Vrb__With_Correct_Values()
	{
		await new Setup().Test01((h, q) => h.HandleAsync(q));
	}

	[Fact]
	public override async Task Test02_Calls_FluentQuery_Where__With_Correct_Values()
	{
		await new Setup().Test02((h, q) => h.HandleAsync(q));
	}

	[Fact]
	public override async Task Test03_Calls_FluentQuery_QuerySingleAsync()
	{
		await new Setup().Test03((h, q) => h.HandleAsync(q));
	}

	[Fact]
	public override async Task Test04_Calls_FluentQuery_QuerySingleAsync__Returns_Result()
	{
		await new Setup().Test04((h, q) => h.HandleAsync(q));
	}
}
