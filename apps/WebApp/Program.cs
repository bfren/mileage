// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Mileage.WebApp;

var app = Jeebs.Apps.Web.MvcApp.Create<App>(args);

var dispatcher = app.Services.GetRequiredService<IDispatcher>();
await dispatcher.DispatchAsync(
	new Mileage.Domain.MigrateToLatest.MigrateToLatestCommand()
);
//await dispatcher.DispatchAsync(
//	new Mileage.Domain.TruncateEverything.TruncateEverythingCommand()
//);
//await dispatcher.DispatchAsync(
//	new Mileage.Domain.InsertTestData.InsertTestDataCommand()
//);

app.Run();
