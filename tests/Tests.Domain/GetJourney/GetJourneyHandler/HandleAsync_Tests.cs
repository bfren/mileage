// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data.Ids;
using Jeebs.Functions;
using Mileage.Persistence.Common.Ids;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetJourney.GetJourney_Tests;

public sealed class HandleAsync_Tests : Abstracts.GetSingle.HandleAsync_Tests
{
	private sealed class Setup : Setup<IJourneyRepository, JourneyEntity, JourneyId, GetJourneyQuery, GetJourneyHandler, JourneyModel>
	{
		public Setup() : base("Journey", false) { }

		internal override GetJourneyHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);

		internal override GetJourneyQuery GetQuery(AuthUserId? userId = null, JourneyId? entityId = null)
		{
			if (userId is not null && entityId is not null)
			{
				return new(userId, entityId);
			}

			return new(IdGen.LongId<AuthUserId>(), IdGen.LongId<JourneyId>());
		}

		internal override JourneyModel NewModel { get; } = new(
			IdGen.LongId<AuthUserId>(), IdGen.LongId<JourneyId>(), Rnd.Lng, Rnd.DateTime, IdGen.LongId<CarId>(), Rnd.Int, Rnd.Int,
			IdGen.LongId<PlaceId>(), ListF.Create(IdGen.LongId<PlaceId>(), IdGen.LongId<PlaceId>()), IdGen.LongId<RateId>()
		);
	}

	[Fact]
	public override async Task Test00_Id_Is_Null__Returns_None_With_NullMsg()
	{
		await new Setup().Test00((h, q) => h.HandleAsync(q));
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
	public override async Task Test05_Calls_FluentQuery_QuerySingleAsync__Same_UserId__Returns_Result()
	{
		await new Setup().Test05((h, q) => h.HandleAsync(q));
	}

	#region Unused

	[Fact]
	public override Task Test04_Calls_FluentQuery_QuerySingleAsync__Different_UserId__Returns_None_With_DoesNotBelongToUserMsg() =>
		Task.FromResult(true);

	#endregion Unused
}
