// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs;
using Jeebs.Cqrs;
using Mileage.WebApp;

// Create app
var (app, log) = Jeebs.Apps.Web.MvcApp.Create<App>(args);
var dispatcher = app.Services.GetRequiredService<IDispatcher>();

// Migrate to latest database version
log.Inf("Migrate database to latest version.");
_ = await dispatcher
	.DispatchAsync(new Mileage.Domain.MigrateToLatest.MigrateToLatestCommand())
	.LogBoolAsync(log);

// Check for user to insert
if (env("MILEAGE_USER_EMAIL") is string email && env("MILEAGE_USER_PASS") is string pass)
{
	var name = env("MILEAGE_USER_NAME") ?? "Default";
	log.Inf("Attempting to create user {Name} with {Email}.", name, email);
	_ = await dispatcher
		.DispatchAsync(new Mileage.Domain.CreateUser.CreateUserQuery(name, email, pass))
		.AuditAsync(
			some: x => log.Inf("Created user {Id}.", x.Value),
			none: log.Msg
		);
}

// Truncate tables and insert test data
if (env("TRUNCATE") == "true")
{
	log.Wrn("Truncating database tables.");
	_ = await dispatcher
		.DispatchAsync(new Mileage.Domain.TruncateEverything.TruncateEverythingCommand())
		.LogBoolAsync(log);

	log.Inf("Inserting test data.");
	_ = await dispatcher
		.DispatchAsync(new Mileage.Domain.InsertTestData.InsertTestDataCommand())
		.LogBoolAsync(log);
}

// Run app
app.Run();

// Get environment variable shorthand
static string? env(string key) =>
	Environment.GetEnvironmentVariable(key);
