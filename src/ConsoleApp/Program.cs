// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
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

// Get dispatchers
var dispatcher = app.Services.GetRequiredService<IDispatcher>();

// ==========================================
//  RUN MIGRATIONS
// ==========================================

// Authentication
log.Inf("Migrate to latest database version.");
await dispatcher.DispatchAsync(
	new Q.MigrateToLatest.MigrateToLatestCommand()
);

// ==========================================
//  INSERT TEST USER
// ==========================================

var userId = await dispatcher.DispatchAsync(
	new Q.CreateUser.CreateUserQuery("Ben", "ben@bcgdesign.com", "fred")
).AuditAsync(
	some: x => log.Dbg("New User: {UserId}.", x),
	none: r => log.Err("Failed to add User: {Reason}.", r)
).UnwrapAsync(
	s => s.Value(() => new())
);

// ==========================================
//  INSERT TEST JOURNEY
// ==========================================

var journeyId = await dispatcher.DispatchAsync(
	new Q.CreateJourney.CreateJourneyQuery(userId, DateOnly.FromDateTime(DateTime.Now), new(), Rnd.Uint, new())
).AuditAsync(
	some: x => log.Dbg("New Journey: {JourneyId}.", x),
	none: r => log.Err("Failed to add Journey: {Reason}.", r)
).UnwrapAsync(
	s => s.Value(() => new())
);

// ==========================================
//  DELETE TEST JOURNEY
// ==========================================

await dispatcher.DispatchAsync(
	new Q.DeleteJourney.DeleteJourneyQuery(journeyId, userId)
).AuditAsync(
	some: x => { if (x) { log.Dbg("Journey deleted."); } else { log.Dbg("Journey not deleted."); } },
	none: r => log.Err("Failed to delete Journey: {Reason}.", r)
);

// ==========================================
//  LOAD SETTINGS
// ==========================================

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

var authDb = app.Services.GetRequiredService<AuthDb>();

var truncate = Task (string table) =>
	authDb.ExecuteAsync($"TRUNCATE TABLE {table};", null, System.Data.CommandType.Text);

await truncate("\"auth\".\"User\"");
await truncate("\"mileage\".\"Journey\"");
