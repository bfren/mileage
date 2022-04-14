// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Domain.DeletePlace.Messages;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.DeletePlace.DeletePlaceHandler_Tests;

public class HandleAsync_Tests : Abstracts.Delete.HandleAsync_Tests
{
	private class Setup : Delete_Setup<IPlaceRepository, PlaceEntity, PlaceId, DeletePlaceCommand, DeletePlaceHandler, PlaceToDelete>
	{
		public Setup() : base("Place") { }

		internal override DeletePlaceHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);

		internal override DeletePlaceCommand GetCommand(AuthUserId? userId = null, PlaceId? entityId = null)
		{
			if (userId is not null && entityId is not null)
			{
				return new(userId, entityId);
			}

			return new(LongId<AuthUserId>(), LongId<PlaceId>());
		}

		internal override PlaceToDelete EmptyModel { get; } = new(LongId<PlaceId>(), Rnd.Lng);
	}

	[Fact]
	public override async Task Test00_Calls_Log_Vrb__With_Query()
	{
		await new Setup().Test00((q, c) => q.HandleAsync(c));
	}

	[Fact]
	public override async Task Test01_Calls_FluentQuery_Where__With_Correct_Values()
	{
		await new Setup().Test01((q, c) => q.HandleAsync(c));
	}

	[Fact]
	public override async Task Test02_Calls_FluentQuery_WhereSingleAsync__Receives_None__Audits_Msg()
	{
		await new Setup().Test02((q, c) => q.HandleAsync(c));
	}

	[Fact]
	public override async Task Test03_Calls_FluentQuery_WhereSingleAsync__Receives_None__Returns_None_With_DoesNotExistMsg()
	{
		await new Setup().Test03<PlaceDoesNotExistMsg>(msg => msg.UserId, msg => msg.PlaceId, (q, c) => q.HandleAsync(c));
	}

	[Fact]
	public override async Task Test04_Calls_FluentQuery_WhereSingleAsync__Receives_Some__Calls_Repo_DeleteAsync__With_Correct_Value()
	{
		await new Setup().Test04((carId, version) => new(carId, version), (q, c) => q.HandleAsync(c));
	}

	public override async Task Test05_Calls_FluentQuery_WhereSingleAsync__Receives_Some__Calls_Repo_DeleteAsync__Returns_Result()
	{
		await new Setup().Test05((q, c) => q.HandleAsync(c));
	}
}
