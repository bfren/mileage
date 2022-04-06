// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

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

pause();

// ==========================================
//  GET LATEST END MILES
// ==========================================

write("TEST GET LATEST END MILES");
var mileage = new List<(uint start, uint end)>();
var number = 10;
for (var i = 0; i < number; i++)
{
	var start = Rnd.UInt;
	var end = start + Rnd.UInt;
	mileage.Add((start, end));
}
mileage.Sort((a, b) => a.start.CompareTo(b.start));

for (var i = 0; i < number; i++)
{
	var (start, end) = mileage[i];
	_ = await dispatcher.DispatchAsync(
		new Q.SaveJourney.SaveJourneyQuery(userId, null, null, Rnd.Date, carId, start, end, placeId, null, rateId)
	);
}

_ = await dispatcher.DispatchAsync(
	new Q.GetLatestEndMiles.GetLatestEndMilesQuery(userId, carId)
).AuditAsync(
	some: x =>
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
	none: r => log.Err("Failed to get latest end miles: {Reason}.", r)
);

_ = await dispatcher.DispatchAsync(
	new Q.SaveJourney.SaveJourneyQuery(userId, carId, mileage.Last().start + Rnd.UInt, placeId)
);
_ = await dispatcher.DispatchAsync(
	new Q.GetLatestEndMiles.GetLatestEndMilesQuery(userId, carId)
).AuditAsync(
	some: x =>
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
	none: r => log.Err("Failed to get latest end miles: {Reason}.", r)
);

// ==========================================
//  GET INCOMPLETE JOURNEYS
// ==========================================

write("INCOMPLETE JOURNEYS");
var incomplete = await dispatcher.DispatchAsync(
	new Q.GetIncompleteJourneys.GetIncompleteJourneysQuery(userId)
).AuditAsync(
	some: x =>
	{
		foreach (var item in x)
		{
			log.Dbg("Incomplete journey: {Journey}", item);
		}
	},
	none: r => log.Err("Unable to get incomplete journeys: {Reason}.", r)
);

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

await dispatcher.DispatchAsync(
	new Q.SaveSettings.SaveSettingsCommand(userId, settings with { DefaultCarId = carId })
).AuditAsync(
	some: x => { if (x) { log.Dbg("Saved settings with {CarId}.", carId); } else { log.Dbg("Settings not saved."); } },
	none: r => log.Err("Failed to save settings: {Reason}.", r)
);

await dispatcher.DispatchAsync(
	new Q.SaveSettings.SaveSettingsCommand(userId, settings with { DefaultFromPlaceId = placeId })
).AuditAsync(
	some: x => { if (x) { log.Dbg("Saved settings with {PlaceId}.", placeId); } else { log.Dbg("Settings not saved."); } },
	none: r => log.Err("Failed to save settings: {Reason}.", r)
);

// ==========================================
//  PAUSE
// ==========================================

pause();

// ==========================================
//  DELETE TEST JOURNEY
// ==========================================

write("DELETE FIRST JOURNEY");
await dispatcher.DispatchAsync(
	new Q.DeleteJourney.DeleteJourneyCommand(userId, journeyId)
).AuditAsync(
	some: x => { if (x) { log.Dbg("Journey deleted."); } else { log.Dbg("Journey not deleted."); } },
	none: r => log.Err("Failed to delete Journey: {Reason}.", r)
);

// ==========================================
//  DELETE TEST RATE
// ==========================================

write("DELETE RATE");
await dispatcher.DispatchAsync(
	new Q.DeleteRate.DeleteRateCommand(userId, rateId)
).AuditAsync(
	some: x => { if (x) { log.Dbg("Rate deleted."); } else { log.Dbg("Rate not deleted."); } },
	none: r => log.Err("Failed to delete Rate: {Reason}.", r)
);

// ==========================================
//  DELETE TEST PLACE
// ==========================================

write("DELETE PLACE");
await dispatcher.DispatchAsync(
	new Q.DeletePlace.DeletePlaceCommand(userId, placeId)
).AuditAsync(
	some: x => { if (x) { log.Dbg("Place deleted."); } else { log.Dbg("Place not deleted."); } },
	none: r => log.Err("Failed to delete Place: {Reason}.", r)
);

// ==========================================
//  DELETE TEST CAR
// ==========================================

write("DELETE CAR");
await dispatcher.DispatchAsync(
	new Q.DeleteCar.DeleteCarCommand(userId, carId)
).AuditAsync(
	some: x => { if (x) { log.Dbg("Car deleted."); } else { log.Dbg("Car not deleted."); } },
	none: r => log.Err("Failed to delete Car: {Reason}.", r)
);

// ==========================================
//  PAUSE
// ==========================================

pause();

// ==========================================
//  TRUNCATE TABLES
// ==========================================

write("TRUNCATE TABLES");
await dispatcher.DispatchAsync(
	new Q.TruncateEverything.TruncateEverythingCommand()
).AuditAsync(
	some: x => { if (x) { log.Dbg("Tables truncated."); } else { log.Dbg("Tables not truncated."); } },
	none: r => log.Err("Failed to truncate tables: {Reason}.", r)
);
