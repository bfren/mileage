// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Domain.GetGetAnnualMileageReportYears;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Domain.GetAnnualMileageReportYears.GetAnnualMileageReportYearsHandler_Tests;

public sealed class GetTaxYear_Tests : Abstracts.TestHandler
{
	private sealed class Setup : Setup<IJourneyRepository, JourneyEntity, JourneyId, GetAnnualMileageReportYearsHandler>
	{
		internal override GetAnnualMileageReportYearsHandler GetHandler(Vars v) =>
			new(v.Repo, v.Log);
	}

	private (GetAnnualMileageReportYearsHandler, Setup.Vars) GetVars() =>
		new Setup().GetVars();

	public static TheoryData<DateTime, int> Returns_Correct_Tax_Year_Data =>
			new()
			{
				{ new DateTime(2024, 04, 05), 2023 },
				{ new DateTime(2024, 04, 06), 2024 },
				{ new DateTime(2024, 01, 01), 2023 },
				{ new DateTime(2024, 12, 31), 2024 },
			};

	[Theory]
	[MemberData(nameof(Returns_Correct_Tax_Year_Data))]
	public void Returns_Correct_Tax_Year(DateTime date, int startYear)
	{
		// Arrange
		var (handler, _) = GetVars();

		// Act
		var result = handler.GetTaxYear(date);

		// Assert
		Assert.Equal(startYear, result.StartYear);
	}
}
