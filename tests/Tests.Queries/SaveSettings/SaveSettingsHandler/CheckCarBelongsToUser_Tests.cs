// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Repositories;

namespace Mileage.Queries.SaveSettings.SaveSettingsHandler_Tests;

public class CheckCarBelongsToUser_Tests : TestHandler<ISettingsRepository, SettingsEntity, SettingsId, SaveSettingsHandler>
{
	public override SaveSettingsHandler GetHandler(Vars v) =>
		new(v.Dispatcher, v.Repo, v.Log);

	[Fact]
	public async Task With_Null_CarId__Returns_True()
	{
		// Arrange
		var (handler, _) = GetVars();

		// Act
		var result = await handler.CheckCarBelongsToUser(null, new(), CancellationToken.None);

		// Assert
		Assert.True(result);
	}
}
