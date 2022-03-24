// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Auth.Data.Clients.PostgreSql;
using Jeebs.Config.Db;
using Jeebs.Cqrs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mileage.Persistence.Common.StrongIds;
using RndF;
using Q = Mileage.Queries;

// ==========================================
//  CONFIGURE
// ==========================================

var (app, log) = Jeebs.Apps.Host.Create(args, (ctx, services) =>
{
	services.AddAuthData<PostgreSqlDbClient>(true);
	services.AddCqrs();
});

// ==========================================
//  BEGIN
// ==========================================

log.Inf("Mileage Console app.");

// ==========================================
//  SETUP
// ==========================================

// Create client
var authDb = app.Services.GetRequiredService<AuthDb>();
var authDbClient = app.Services.GetRequiredService<PostgreSqlDbClient>();

// Get config
var config = app.Services.GetRequiredService<IOptions<DbConfig>>().Value;
var dbConnection = config.GetConnection("rpi").ConnectionString;
log.Dbg(dbConnection);

// Get query dispatcher
var query = app.Services.GetRequiredService<IQueryDispatcher>();

// ==========================================
//  RUN MIGRATIONS
// ==========================================

// Authentication
log.Inf("Migrate to latest version.");
authDbClient.MigrateToLatest(dbConnection);

// ==========================================
//  INSERT TEST USER
// ==========================================

var userId = await query.DispatchAsync<Q.CreateUser.CreateUserQuery, AuthUserId>(
	new("Ben", "ben@bcgdesign.com", "fred")
).AuditAsync(
	some: x => log.Dbg("New User: {UserId}.", x),
	none: r => log.Err("Failed to add User: {Reason}.", r)
).UnwrapAsync(
	s => s.Value(() => new())
);

// ==========================================
//  INSERT TEST JOURNEY
// ==========================================

var journeyId = await query.DispatchAsync<Q.CreateJourney.CreateJourneyQuery, JourneyId>(
	new(userId, DateOnly.FromDateTime(DateTime.Now), new(), Rnd.Uint, new())
).AuditAsync(
	some: x => log.Dbg("New Journey: {JourneyId}.", x),
	none: r => log.Err("Failed to add Journey: {Reason}.", r)
).UnwrapAsync(
	s => s.Value(() => new())
);

// ==========================================
//  TRUNCATE TABLES
// ==========================================

var truncate = Task (string table) =>
	authDb.ExecuteAsync($"TRUNCATE TABLE {table};", null, System.Data.CommandType.Text);

await truncate("\"auth\".\"User\"");
