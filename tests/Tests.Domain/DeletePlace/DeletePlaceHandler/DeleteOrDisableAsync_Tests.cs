// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Domain.DeletePlace.Messages;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.DeletePlace.DeletePlaceHandler_Tests;

public class DeleteOrDisableAsync_Tests : Abstracts.DeleteOrDisable.DeleteOrDisableAsync_Tests
{
	private sealed class Setup : Setup<IPlaceRepository, PlaceEntity, PlaceId, DeletePlaceCommand, DeletePlaceHandler, PlaceToDeleteModel>
	{
		internal override DeletePlaceHandler GetHandler(Vars v) =>
			new(v.Cache, v.Dispatcher, v.Repo, v.Log);

		internal override PlaceToDeleteModel EmptyModel { get; } = new(LongId<PlaceId>(), Rnd.Lng, Rnd.Flip);
	}

	[Fact]
	public override async Task Test00_Calls_FluentQuery_Where__With_Correct_Values()
	{
		await new Setup().Test00(h => h.DeleteOrDisableAsync);
	}

	[Fact]
	public override async Task Test01_Calls_FluentQuery_WhereSingleAsync__Receives_None__Audits_Msg()
	{
		await new Setup().Test01(h => h.DeleteOrDisableAsync);
	}

	[Fact]
	public override async Task Test02_Calls_FluentQuery_WhereSingleAsync__Receives_None__Returns_None_With_DoesNotExistMsg()
	{
		await new Setup().Test02<PlaceDoesNotExistMsg>(h => h.DeleteOrDisableAsync);
	}

	[Fact]
	public override async Task Test03_Is_Delete__Calls_Repo_DeleteAsync__With_Correct_Values()
	{
		await new Setup().Test03((i, v, d) => new PlaceToDeleteModel(i, v, d), h => h.DeleteOrDisableAsync);
	}

	[Fact]
	public override async Task Test04_Is_Disable__Calls_Repo_UpdateAsync__With_Correct_Values()
	{
		await new Setup().Test04((i, v, d) => new PlaceToDeleteModel(i, v, d), h => h.DeleteOrDisableAsync);
	}

	[Fact]
	public override async Task Test05_Is_None__Returns_None_With_CannotBeDeletedMsg()
	{
		await new Setup().Test05<PlaceCannotBeDeletedMsg>(h => h.DeleteOrDisableAsync);
	}
}
