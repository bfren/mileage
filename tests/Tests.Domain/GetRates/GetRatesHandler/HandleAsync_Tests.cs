// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Data.Enums;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetRates.GetRates_Tests;

public class HandleAsync_Tests : Abstracts.GetEnumerable.HandleAsync_Tests
{
	private class Setup : Setup<IRateRepository, RateEntity, RateId, GetRatesQuery, GetRatesHandler, RatesModel>
	{
		public Setup() : base("Rates") { }

		internal override GetRatesHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);

		internal override (GetRatesQuery, AuthUserId) GetQuery(AuthUserId? userId = null)
		{
			if (userId is null)
			{
				userId = LongId<AuthUserId>();
			}

			return (new(userId, true), userId);
		}

		internal override RatesModel NewModel { get; } = new(LongId<RateId>(), Rnd.Flt, Rnd.Flip);
	}

	[Fact]
	public override async Task Test00_Id_Is_Null__Returns_None_With_NullMsg()
	{
		await new Setup().Test00<Messages.UserIdIsNullMsg>((h, q) => h.HandleAsync(q));
	}

	[Fact]
	public override async Task Test01_Calls_Log_Vrb__With_Correct_Values()
	{
		await new Setup().Test01((h, q) => h.HandleAsync(q));
	}

	[Fact]
	public override async Task Test02_Calls_FluentQuery_Where__With_Correct_Values()
	{
		await new Setup().Test02_WithIsDisabled((u, d) => new(u, d), (h, q) => h.HandleAsync(q));
	}

	[Fact]
	public override async Task Test03_Calls_FluentQuery_Sort__With_Correct_Values()
	{
		await new Setup().Test03_WithIsDisabled(x => x.AmountPerMileGBP, SortOrder.Ascending, (h, q) => h.HandleAsync(q));
	}

	[Fact]
	public override async Task Test04_Calls_FluentQuery_QueryAsync()
	{
		await new Setup().Test04((h, q) => h.HandleAsync(q));
	}

	[Fact]
	public override async Task Test05_Calls_FluentQuery_QueryAsync__Returns_Result()
	{
		await new Setup().Test05((h, q) => h.HandleAsync(q));
	}
}
