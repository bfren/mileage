// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using System.Data;
using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Microsoft.Extensions.DependencyInjection;
using Mileage.Domain;
using RndF;
using Q = Mileage.Domain;

// ==========================================
//  CONFIGURE
// ==========================================

var (app, log) = Jeebs.Apps.Host.Create(args, (ctx, services) => services.AddData());

// ==========================================
//  BEGIN
// ==========================================

log.Inf("Mileage Console app.");

// ==========================================
//  SETUP
// ==========================================

var dispatcher = app.Services.GetRequiredService<IDispatcher>();
static void write(string text)
{
	var pad = new string('=', text.Length + 6);
	Console.WriteLine();
	Console.WriteLine(pad);
	Console.WriteLine($"== {text} ==");
	Console.WriteLine(pad);
}

// ==========================================
//  RUN MIGRATIONS
// ==========================================

write("MIGRATIONS");
log.Inf("Migrate to latest database version.");
await dispatcher.DispatchAsync(
	new Q.MigrateToLatest.MigrateToLatestCommand()
);

// ==========================================
//  INSERT TEST USER
// ==========================================

write("INSERT USER");
var name = Rnd.Str;
var email = "info@bfren.dev";
var pass = "fred";
var userId = await dispatcher.DispatchAsync(
	new Q.CreateUser.CreateUserQuery(name, email, pass)
).AuditAsync(
	some: x => log.Dbg("New User: {UserId}.", x),
	none: r => log.Err("Failed to add User: {Reason}.", r)
).UnwrapAsync(
	s => s.Value(() => new())
);

// ==========================================
//  INSERT TEST CAR
// ==========================================

write("INSERT CAR");
var carDescription = Rnd.Str;
var carId = await dispatcher.DispatchAsync(
	new Q.SaveCar.SaveCarQuery(userId, carDescription)
).AuditAsync(
	some: x => log.Dbg("New Car: {CarId}.", x),
	none: r => log.Err("Failed to add Car: {Reason}.", r)
).UnwrapAsync(
	s => s.Value(() => new())
);

// ==========================================
//  INSERT TEST PLACE
// ==========================================

write("INSERT PLACE");
var placeDescription = Rnd.Str;
var placeId = await dispatcher.DispatchAsync(
	new Q.SavePlace.SavePlaceQuery(userId, placeDescription)
).AuditAsync(
	some: x => log.Dbg("New Place: {PlaceId}.", x),
	none: r => log.Err("Failed to add Place: {Reason}.", r)
).UnwrapAsync(
	s => s.Value(() => new())
);

// ==========================================
//  INSERT TEST RATE
// ==========================================

write("INSERT PLACE");
var amount = Rnd.NumberF.GetSingle(min: 0.1f, max: 0.9f);
var rateId = await dispatcher.DispatchAsync(
	new Q.SaveRate.SaveRateQuery(userId, amount)
).AuditAsync(
	some: x => log.Dbg("New Rate: {RateId}.", x),
	none: r => log.Err("Failed to add Rate: {Reason}.", r)
).UnwrapAsync(
	s => s.Value(() => new())
);

// ==========================================
//  INSERT TEST JOURNEY
// ==========================================

write("INSERT JOURNEY");
var journeyId = await dispatcher.DispatchAsync(
	new Q.SaveJourney.SaveJourneyQuery(userId, carId, Rnd.UInt, placeId)
).AuditAsync(
	some: x => log.Dbg("New Journey: {JourneyId}.", x),
	none: r => log.Err("Failed to add Journey: {Reason}.", r)
).UnwrapAsync(
	s => s.Value(() => new())
);

// ==========================================
//  PAUSE
// ==========================================

write("PAUSE");
Console.WriteLine("Press any key when ready.");
Console.Read();

// ==========================================
//  DELETE TEST JOURNEY
// ==========================================

write("DELETE JOURNEY");
await dispatcher.DispatchAsync(
	new Q.DeleteJourney.DeleteJourneyQuery(userId, journeyId)
).AuditAsync(
	some: x => { if (x) { log.Dbg("Journey deleted."); } else { log.Dbg("Journey not deleted."); } },
	none: r => log.Err("Failed to delete Journey: {Reason}.", r)
);

// ==========================================
//  DELETE TEST RATE
// ==========================================

write("DELETE RATE");
//await dispatcher.DispatchAsync(
//	new Q.DeleteJourney.DeleteJourneyQuery(journeyId, userId)
//).AuditAsync(
//	some: x => { if (x) { log.Dbg("Journey deleted."); } else { log.Dbg("Journey not deleted."); } },
//	none: r => log.Err("Failed to delete Journey: {Reason}.", r)
//);

// ==========================================
//  DELETE TEST PLACE
// ==========================================

write("DELETE PLACE");
//await dispatcher.DispatchAsync(
//	new Q.DeleteJourney.DeleteJourneyQuery(journeyId, userId)
//).AuditAsync(
//	some: x => { if (x) { log.Dbg("Journey deleted."); } else { log.Dbg("Journey not deleted."); } },
//	none: r => log.Err("Failed to delete Journey: {Reason}.", r)
//);

// ==========================================
//  DELETE TEST CAR
// ==========================================

write("DELETE CAR");
//await dispatcher.DispatchAsync(
//	new Q.DeleteJourney.DeleteJourneyQuery(journeyId, userId)
//).AuditAsync(
//	some: x => { if (x) { log.Dbg("Journey deleted."); } else { log.Dbg("Journey not deleted."); } },
//	none: r => log.Err("Failed to delete Journey: {Reason}.", r)
//);

// ==========================================
//  LOAD SETTINGS
// ==========================================

write("LOAD SETTINGS");
var settings = await dispatcher.DispatchAsync(
	new Q.LoadSettings.LoadSettingsQuery(userId)
).UnwrapAsync(
	x => x.Value(() => throw new InvalidOperationException())
);
log.Dbg("Settings for User {UserId}: {Settings}", userId.Value, settings);

// ==========================================
//  SAVE SETTINGS
// ==========================================

// ==========================================
//  TRUNCATE TABLES
// ==========================================

write("TRUNCATE TABLES");
var authDb = app.Services.GetRequiredService<AuthDb>();

var truncate = Task (string table, IDbTransaction transaction) =>
	authDb.ExecuteAsync($"TRUNCATE TABLE {table};", null, System.Data.CommandType.Text, transaction);

using (var w = authDb.UnitOfWork)
{
	await truncate("\"auth\".\"User\"", w.Transaction);
	await truncate("\"mileage\".\"Car\"", w.Transaction);
	await truncate("\"mileage\".\"Journey\"", w.Transaction);
	await truncate("\"mileage\".\"Place\"", w.Transaction);
	await truncate("\"mileage\".\"Rate\"", w.Transaction);
	await truncate("\"mileage\".\"Settings\"", w.Transaction);
}
