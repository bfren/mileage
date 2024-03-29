// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Domain.GetCar.Messages;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetCar.GetCar_Tests;

public sealed class HandleAsync_Tests : Abstracts.GetSingle.HandleAsync_Tests
{
	private sealed class Setup : Setup<ICarRepository, CarEntity, CarId, GetCarQuery, GetCarHandler, CarModel>
	{
		public Setup() : base("Car", true) { }

		internal override GetCarHandler GetHandler(Vars v) =>
			new(v.Cache, v.Repo, v.Log);

		internal override GetCarQuery GetQuery(AuthUserId? userId = null, CarId? entityId = null)
		{
			if (userId is not null && entityId is not null)
			{
				return new(userId, entityId);
			}

			return new(LongId<AuthUserId>(), LongId<CarId>());
		}

		internal override CarModel NewModel { get; } = new(LongId<AuthUserId>(), LongId<CarId>(), Rnd.Lng, Rnd.Str, Rnd.Str, Rnd.Flip);
	}

	[Fact]
	public override async Task Test00_Id_Is_Null__Returns_None_With_NullMsg()
	{
		await new Setup().Test00<CarIdIsNullMsg>((h, q) => h.HandleAsync(q));
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
	public override async Task Test04_Calls_FluentQuery_QuerySingleAsync__Different_UserId__Returns_None_With_DoesNotBelongToUserMsg()
	{
		await new Setup().Test04<CarDoesNotBelongToUserMsg>((h, q) => h.HandleAsync(q));
	}

	[Fact]
	public override async Task Test05_Calls_FluentQuery_QuerySingleAsync__Same_UserId__Returns_Result()
	{
		await new Setup().Test05((h, q) => h.HandleAsync(q));
	}
}
