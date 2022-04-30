// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Domain.CheckCarCanBeDeleted;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.DeleteCar.DeleteCarHandler_Tests;

public class HandleAsync_Tests : Abstracts.DeleteOrDisable.HandleAsync_Tests
{
	private class Setup : Setup<ICarRepository, CarEntity, CarId, DeleteCarCommand, DeleteCarHandler, CarToDelete, CheckCarCanBeDeletedQuery>
	{
		public Setup() : base("Car") { }

		internal override DeleteCarHandler GetHandler(Vars v) =>
			new(v.Cache, v.Repo, v.Dispatcher, v.Log);

		internal override DeleteCarCommand GetCommand(AuthUserId? userId = null, CarId? entityId = null)
		{
			if (userId is not null && entityId is not null)
			{
				return new(userId, entityId);
			}

			return new(LongId<AuthUserId>(), LongId<CarId>());
		}

		internal override CarToDelete EmptyModel { get; } = new(LongId<CarId>(), Rnd.Lng, Rnd.Flip);
	}

	[Fact]
	public override async Task Test00_Calls_Log_Vrb__With_Query()
	{
		await new Setup().Test00((h, c, d) => h.HandleAsync(c, d));
	}

	[Fact]
	public override async Task Test01_Dispatches_Check_Item_Can_Be_Deleted_Query__With_Correct_Values()
	{
		await new Setup().Test01((h, c, d) => h.HandleAsync(c, d));
	}

	[Fact]
	public override async Task Test02_Calls_Delete_Or_Disable__Receives_Some_True__Removes_Item_From_Cache()
	{
		await new Setup().Test02((h, c, d) => h.HandleAsync(c, d));
	}

	[Fact]
	public override async Task Test03_Calls_Delete_Or_Disable__Receives_Some_False__Leaves_Cache_Alone()
	{
		await new Setup().Test03((h, c, d) => h.HandleAsync(c, d));
	}

	[Fact]
	public override async Task Test04_Calls_Delete_Or_Disable__Receives_None__Leaves_Cache_Alone()
	{
		await new Setup().Test04((h, c, d) => h.HandleAsync(c, d));
	}

	[Fact]
	public override async Task Test05_Calls_Delete_Or_Disable__Returns_Result()
	{
		await new Setup().Test05((h, c, d) => h.HandleAsync(c, d));
	}
}
