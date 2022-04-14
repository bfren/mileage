// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Domain.GetFromPlace;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;
using static Mileage.Domain.GetFromPlace.GetFromPlaceHandler.M;

namespace Mileage.Domain.GetPlace.GetFromPlace_Tests;

public class HandleAsync_Tests : Abstracts.GetSingle.HandleAsync_Tests
{
	private class Setup : GetSingle_Setup<IPlaceRepository, PlaceEntity, PlaceId, GetFromPlaceQuery, GetFromPlaceHandler, GetFromPlaceModel>
	{
		public Setup() : base("Place") { }

		internal override GetFromPlaceHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);

		internal override GetFromPlaceQuery GetQuery(AuthUserId? userId = null, PlaceId? entityId = null)
		{
			if (userId is not null && entityId is not null)
			{
				return new(userId, entityId);
			}

			return new(LongId<AuthUserId>(), LongId<PlaceId>());
		}

		internal override GetFromPlaceModel NewModel { get; } = new(LongId<PlaceId>(), Rnd.Str);
	}

	[Fact]
	public override async Task Test00_Id_Is_Null__Returns_None_With_NullMsg()
	{
		await new Setup().Test00<PlaceIdIsNullMsg>((h, q) => h.HandleAsync(q));
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
