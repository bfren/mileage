// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.Domain.SaveJourney.UpdateJourneyStartMilesHandler_Tests;

public sealed class HandleAsync_Tests : Abstracts.UpdateJourney.HandleAsync_Tests
{
	private sealed class Setup : Setup<UpdateJourneyStartMilesCommand, UpdateJourneyStartMilesHandler>
	{
		public Setup() : base("Start Miles") { }

		internal override UpdateJourneyStartMilesCommand GetCommand(AuthUserId? userId = null)
		{
			if (userId is null)
			{
				userId = LongId<AuthUserId>();
			}

			return new(userId, LongId<JourneyId>(), Rnd.Lng, Rnd.Int);
		}

		internal override UpdateJourneyStartMilesHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);
	}

	[Fact]
	public override async Task Test00_Calls_Log_Vrb__With_Correct_Values()
	{
		await new Setup().Test00((h, c) => h.HandleAsync(c));
	}

	[Fact]
	public override async Task Test01_Calls_Repo_UpdateAsync__With_Correct_Values()
	{
		await new Setup().Test01((h, c) => h.HandleAsync(c));
	}

	[Fact]
	public override async Task Test02_Calls_Repo_Update_Async__Returns_Result()
	{
		await new Setup().Test02((h, c) => h.HandleAsync(c));
	}
}
