// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveSettings.UpdateDefaultRateHandler_Tests;

public sealed class HandleAsync_Tests : Abstracts.UpdateSettings.HandleAsync_Tests
{
	private sealed class Setup : Setup<UpdateDefaultRateCommand, UpdateDefaultRateHandler, RateId>
	{
		internal override UpdateDefaultRateCommand GetCommand(AuthUserId? userId = null, RateId? itemId = null) =>
			new(userId ?? LongId<AuthUserId>(), LongId<SettingsId>(), Rnd.Lng, itemId);

		internal override UpdateDefaultRateHandler GetHandler(Vars v) =>
			new(v.Dispatcher, v.Repo, v.Log);

		public Setup() : base("Rate") { }
	}

	[Fact]
	public override async Task Test00_ItemId_Is_Not_Null__Checks_Item_Belongs_To_User__Receives_None__Returns_None_With_SaveSettingsCheckFailedMsg()
	{
		await new Setup().Test00((h, c) => h.HandleAsync(c));
	}

	[Fact]
	public override async Task Test01_ItemId_Is_Not_Null__Checks_Item_Belongs_To_User__Receives_Some_False__Returns_None_With_SaveSettingsCheckFailedMsg()
	{
		await new Setup().Test01((h, c) => h.HandleAsync(c));
	}

	[Fact]
	public override async Task Test02_ItemId_Is_Not_Null__Checks_Item_Belongs_To_User__Receives_Some_True__Calls_Log_Vrb__With_Correct_Values()
	{
		await new Setup().Test02((h, c) => h.HandleAsync(c));
	}

	[Fact]
	public override async Task Test03_ItemId_Is_Not_Null__Checks_Item_Belongs_To_User__Receives_Some_True__Calls_Settings_UpdateAsync__With_Correct_Values()
	{
		await new Setup().Test03((h, c) => h.HandleAsync(c));
	}

	[Fact]
	public override async Task Test04_ItemId_Is_Null__Calls_Log_Vrb__With_Correct_Values()
	{
		await new Setup().Test04((h, c) => h.HandleAsync(c));
	}

	[Fact]
	public override async Task Test05_ItemId_Is_Null__Calls_Settings_UpdateAsync__With_Correct_Values()
	{
		await new Setup().Test05((h, c) => h.HandleAsync(c));
	}
}
