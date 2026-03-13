// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Microsoft.Extensions.DependencyInjection;
using Mileage.Domain;
using Mileage.Persistence.Common.Ids;
using RndF;
using Wrap.Caching;
using Q = Mileage.Domain;

// ==========================================
//  CONFIGURE
// ==========================================

var (app, log) = Jeebs.Apps.Host.Create(args, (ctx, services) =>
{
	_ = services.AddData();
	_ = services.AddMemoryCache()
		.AddWrapCache<CarId>()
		.AddWrapCache<PlaceId>()
		.AddWrapCache<RateId>();
});

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

static void pause()
{
	write("PAUSE");
	Console.WriteLine("Press any key when ready.");
	_ = Console.ReadLine();
}

// ==========================================
//  RUN MIGRATIONS
// ==========================================

write("MIGRATIONS");
log.Inf("Migrate to latest database version.");
await dispatcher.SendAsync(
	new Q.MigrateToLatest.MigrateToLatestCommand()
);

// ==========================================
//  INSERT TEST USER
// ==========================================

write("INSERT USER");
var name = Rnd.Str;
var email = "info@bfren.dev";
var pass = "fred";
var userId = await dispatcher.SendAsync(
	new Q.CreateUser.CreateUserQuery(name, email, pass)
).AuditAsync(
	fOk: x => log.Dbg("New User: {UserId}.", x),
	fFail: r => log.Err("Failed to add User: {Reason}.", r)
).UnwrapAsync();

// ==========================================
//  INSERT TEST CAR
// ==========================================

write("INSERT CAR");
var carDescription = Rnd.Str;
var carId = await dispatcher.SendAsync(
	new Q.SaveCar.SaveCarQuery(userId, carDescription)
).AuditAsync(
	fOk: x => log.Dbg("New Car: {CarId}.", x),
	fFail: r => log.Err("Failed to add Car: {Reason}.", r)
).UnwrapAsync();

// ==========================================
//  INSERT TEST PLACE
// ==========================================

write("INSERT PLACE");
var placeDescription = Rnd.Str;
var placeId = await dispatcher.SendAsync(
	new Q.SavePlace.SavePlaceQuery(userId, placeDescription)
).AuditAsync(
	fOk: x => log.Dbg("New Place: {PlaceId}.", x),
	fFail: r => log.Err("Failed to add Place: {Reason}.", r)
).UnwrapAsync();

// ==========================================
//  INSERT TEST RATE
// ==========================================

write("INSERT PLACE");
var amount = Rnd.NumberF.GetSingle(min: 0.1f, max: 0.9f);
var rateId = await dispatcher.SendAsync(
	new Q.SaveRate.SaveRateQuery(userId, amount)
).AuditAsync(
	fOk: x => log.Dbg("New Rate: {RateId}.", x),
	fFail: r => log.Err("Failed to add Rate: {Reason}.", r)
).UnwrapAsync();

// ==========================================
//  INSERT TEST JOURNEY
// ==========================================

write("INSERT JOURNEY");
var journeyId = await dispatcher.SendAsync(
	new Q.SaveJourney.SaveJourneyQuery(userId, carId, Rnd.UInt32, placeId)
).AuditAsync(
	fOk: x => log.Dbg("New Journey: {JourneyId}.", x),
	fFail: r => log.Err("Failed to add Journey: {Reason}.", r)
).UnwrapAsync();

// ==========================================
//  PAUSE
// ==========================================

pause();

// ==========================================
//  GET LATEST END MILES
// ==========================================

write("TEST GET LATEST END MILES");
var mileage = new List<(uint start, uint end)>();
var number = 10;
for (var i = 0; i < number; i++)
{
	var start = Rnd.UInt32;
	var end = start + Rnd.UInt32;
	mileage.Add((start, end));
}
mileage.Sort((a, b) => a.start.CompareTo(b.start));

for (var i = 0; i < number; i++)
{
	var (start, end) = mileage[i];
	_ = await dispatcher.SendAsync(
		new Q.SaveJourney.SaveJourneyQuery(userId, null, null, Rnd.DateTime, carId, start, end, placeId, null, rateId)
	);
}

_ = await dispatcher.SendAsync(
	new Q.GetLatestEndMiles.GetLatestEndMilesQuery(userId, carId)
).AuditAsync(
	fOk: x =>
	{
		if (x == mileage.Last().end)
		{
			log.Dbg("Latest end miles: {EndMiles}.", x);
		}
		else
		{
			log.Err("End mileage incorrect: {EndMiles}.", x);
		}
	},
	fFail: r => log.Err("Failed to get latest end miles: {Reason}.", r)
);

_ = await dispatcher.SendAsync(
	new Q.SaveJourney.SaveJourneyQuery(userId, carId, mileage.Last().start + Rnd.UInt32, placeId)
);
_ = await dispatcher.SendAsync(
	new Q.GetLatestEndMiles.GetLatestEndMilesQuery(userId, carId)
).AuditAsync(
	fOk: x =>
	{
		if (x == 0)
		{
			log.Dbg("Latest end miles is not set.");
		}
		else
		{
			log.Err("End milage should be zero: {EndMiles}.", x);
		}
	},
	fFail: r => log.Err("Failed to get latest end miles: {Reason}.", r)
);

// ==========================================
//  GET INCOMPLETE JOURNEYS
// ==========================================

write("INCOMPLETE JOURNEYS");
var incomplete = await dispatcher.SendAsync(
	new Q.GetIncompleteJourneys.GetIncompleteJourneysQuery(userId)
).AuditAsync(
	fOk: x =>
	{
		foreach (var item in x)
		{
			log.Dbg("Incomplete journey: {Journey}", item);
		}
	},
	fFail: r => log.Err("Unable to get incomplete journeys: {Reason}.", r)
);

// ==========================================
//  LOAD SETTINGS
// ==========================================

write("LOAD SETTINGS");
var settings = await dispatcher.SendAsync(
	new Q.LoadSettings.LoadSettingsQuery(userId)
).UnwrapAsync();
log.Dbg("Settings for User {UserId}: {Settings}", userId.Value, settings);

// ==========================================
//  SAVE SETTINGS
// ==========================================

await dispatcher.SendAsync(
	new Q.SaveSettings.SaveSettingsCommand(userId, settings with { DefaultCarId = carId })
).AuditAsync(
	fOk: x => { if (x) { log.Dbg("Saved settings with {CarId}.", carId); } else { log.Dbg("Settings not saved."); } },
	fFail: r => log.Err("Failed to save settings: {Reason}.", r)
);

await dispatcher.SendAsync(
	new Q.SaveSettings.SaveSettingsCommand(userId, settings with { DefaultFromPlaceId = placeId })
).AuditAsync(
	fOk: x => { if (x) { log.Dbg("Saved settings with {PlaceId}.", placeId); } else { log.Dbg("Settings not saved."); } },
	fFail: r => log.Err("Failed to save settings: {Reason}.", r)
);

// ==========================================
//  PAUSE
// ==========================================

pause();

// ==========================================
//  DELETE TEST JOURNEY
// ==========================================

write("DELETE FIRST JOURNEY");
await dispatcher.SendAsync(
	new Q.DeleteJourney.DeleteJourneyCommand(userId, journeyId)
).AuditAsync(
	fOk: x => { if (x) { log.Dbg("Journey deleted."); } else { log.Dbg("Journey not deleted."); } },
	fFail: r => log.Err("Failed to delete Journey: {Reason}.", r)
);

// ==========================================
//  DELETE TEST RATE
// ==========================================

write("DELETE RATE");
await dispatcher.SendAsync(
	new Q.DeleteRate.DeleteRateCommand(userId, rateId)
).AuditAsync(
	fOk: x => { if (x) { log.Dbg("Rate deleted."); } else { log.Dbg("Rate not deleted."); } },
	fFail: r => log.Err("Failed to delete Rate: {Reason}.", r)
);

// ==========================================
//  DELETE TEST PLACE
// ==========================================

write("DELETE PLACE");
await dispatcher.SendAsync(
	new Q.DeletePlace.DeletePlaceCommand(userId, placeId)
).AuditAsync(
	fOk: x => { if (x) { log.Dbg("Place deleted."); } else { log.Dbg("Place not deleted."); } },
	fFail: r => log.Err("Failed to delete Place: {Reason}.", r)
);

// ==========================================
//  DELETE TEST CAR
// ==========================================

write("DELETE CAR");
await dispatcher.SendAsync(
	new Q.DeleteCar.DeleteCarCommand(userId, carId)
).AuditAsync(
	fOk: x => { if (x) { log.Dbg("Car deleted."); } else { log.Dbg("Car not deleted."); } },
	fFail: r => log.Err("Failed to delete Car: {Reason}.", r)
);

// ==========================================
//  PAUSE
// ==========================================

pause();

// ==========================================
//  TRUNCATE TABLES
// ==========================================

write("TRUNCATE TABLES");
await dispatcher.SendAsync(
	new Q.TruncateEverything.TruncateEverythingCommand()
).AuditAsync(
	fOk: x => { if (x) { log.Dbg("Tables truncated."); } else { log.Dbg("Tables not truncated."); } },
	fFail: r => log.Err("Failed to truncate tables: {Reason}.", r)
);
