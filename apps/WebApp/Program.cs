// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Mileage.WebApp;

// Create app
var (app, log) = Jeebs.Apps.Web.MvcApp.Create<App>(args);
var dispatcher = app.Services.GetRequiredService<IDispatcher>();

// Migrate to latest database version
log.Inf("Migrate database to latest version.");
_ = await dispatcher.DispatchAsync(
	new Mileage.Domain.MigrateToLatest.MigrateToLatestCommand()
);

#if DEBUG
// Truncate database tables and insert fresh test data
if (app.Environment.IsEnvironment("Truncate"))
{
	log.Wrn("Truncating database tables.");
	_ = await dispatcher.DispatchAsync(
		new Mileage.Domain.TruncateEverything.TruncateEverythingCommand()
	);

	log.Inf("Inserting test data.");
	_ = await dispatcher.DispatchAsync(
		new Mileage.Domain.InsertTestData.InsertTestDataCommand()
	);
}
#endif

// Run app
app.Run();
