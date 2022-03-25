// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Microsoft.Extensions.DependencyInjection;
using Mileage.Queries;
using RndF;
using Q = Mileage.Queries;

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
var command = app.Services.GetRequiredService<ICommandDispatcher>();
var query = app.Services.GetRequiredService<IQueryDispatcher>();

// ==========================================
//  RUN MIGRATIONS
// ==========================================

// Authentication
log.Inf("Migrate to latest database version.");
await command.DispatchAsync(
	new Q.MigrateToLatest.MigrateToLatestCommand()
);

// ==========================================
//  INSERT TEST USER
// ==========================================

var userId = await query.DispatchAsync(
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

var journeyId = await query.DispatchAsync(
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

await query.DispatchAsync(
	new Q.DeleteJourney.DeleteJourneyQuery(journeyId, userId)
).AuditAsync(
	some: x => { if (x) { log.Dbg("Journey deleted."); } else { log.Dbg("Journey not deleted."); } },
	none: r => log.Err("Failed to delete Journey: {Reason}.", r)
);

// ==========================================
//  TRUNCATE TABLES
// ==========================================

var authDb = app.Services.GetRequiredService<AuthDb>();

var truncate = Task (string table) =>
	authDb.ExecuteAsync($"TRUNCATE TABLE {table};", null, System.Data.CommandType.Text);

await truncate("\"auth\".\"User\"");
await truncate("\"mileage\".\"Journey\"");
