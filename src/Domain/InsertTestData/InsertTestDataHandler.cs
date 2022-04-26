// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Jeebs.Cqrs;
using Jeebs.Logging;
using Mileage.Domain.CreateUser;
using Mileage.Domain.SaveCar.Internals;
using Mileage.Domain.SaveJourney.Internals;
using Mileage.Domain.SavePlace.Internals;
using Mileage.Domain.SaveRate.Internals;
using Mileage.Domain.SaveSettings;
using Mileage.Persistence.Common.StrongIds;
using RndF;

namespace Mileage.Domain.InsertTestData;

/// <summary>
/// Insert test data
/// </summary>
internal sealed class InsertTestDataHandler : CommandHandler<InsertTestDataCommand>
{
	private IDispatcher Dispatcher { get; init; }

	private ILog<InsertTestDataHandler> Log { get; init; }

	/// <summary>
	/// Inject dependencies
	/// </summary>
	/// <param name="dispatcher"></param>
	/// <param name="log"></param>
	public InsertTestDataHandler(IDispatcher dispatcher, ILog<InsertTestDataHandler> log) =>
		(Dispatcher, Log) = (dispatcher, log);

	/// <summary>
	/// Insert various pieces of test data
	/// </summary>
	/// <param name="command"></param>
	public override async Task<Maybe<bool>> HandleAsync(InsertTestDataCommand command)
	{
		Log.Inf("Inserting test data.");
		var q = from u0 in Dispatcher.DispatchAsync(new CreateUserQuery("bf", "info@bfren.dev", "fred"))
				from c0 in Dispatcher.DispatchAsync(new CreateCarQuery(u0, Rnd.Str, Rnd.Str))
				from c1 in Dispatcher.DispatchAsync(new CreateCarQuery(u0, Rnd.Str, Rnd.Str))
				from p0 in Dispatcher.DispatchAsync(new CreatePlaceQuery(u0, Rnd.Str, null))
				from p1 in Dispatcher.DispatchAsync(new CreatePlaceQuery(u0, Rnd.Str, null))
				from p2 in Dispatcher.DispatchAsync(new CreatePlaceQuery(u0, Rnd.Str, null))
				from p3 in Dispatcher.DispatchAsync(new CreatePlaceQuery(u0, Rnd.Str, null))
				from r0 in Dispatcher.DispatchAsync(new CreateRateQuery(u0, Rnd.NumberF.GetSingle(min: 0.1f, max: 0.9f)))
				from r1 in Dispatcher.DispatchAsync(new CreateRateQuery(u0, Rnd.NumberF.GetSingle(min: 0.1f, max: 0.9f)))
				from _ in insertJourneys(u0, c0, c1, p0, p1, new[] { p2, p3 }, r0, r1)
				from s0 in Dispatcher.DispatchAsync(new SaveSettingsCommand(u0, new(new(), 0L, c0, null, null)))
				select true;

		return await q.AuditAsync(
			none: r => Log.Msg(r, LogLevel.Error)
		);

		async Task<Maybe<IEnumerable<JourneyId>>> insertJourneys(
			AuthUserId userId,
			CarId carId0, CarId carId1,
			PlaceId fromPlaceId0, PlaceId fromPlaceId1,
			PlaceId[] toPlaceIds,
			RateId rateId0, RateId rateId1
		)
		{
			Log.Inf("Inserting test journeys.");
			var mileage = new List<(uint start, uint? end)>();

			var miles = Rnd.NumberF.GetUInt32(20000, 30000);
			var date = DateTime.Today;
			const int number = 100;

			// Incomplete journey
			mileage.Add((miles, null));

			// Generate random mileage numbers
			for (var i = 0; i < number; i++)
			{
				var start = miles - Rnd.NumberF.GetUInt32(5, 50);
				mileage.Add((start, miles));
				miles = start;
			}

			// Create each journey
			var journeyIds = new List<JourneyId>();
			for (var i = 0; i < number; i++)
			{
				var carId = Rnd.Flip ? carId0 : carId1;
				var fromPlaceId = Rnd.Flip ? fromPlaceId0 : fromPlaceId1;
				var rateId = Rnd.Flip ? rateId0 : rateId1;
				var (start, end) = mileage[i];

				_ = await Dispatcher
					.DispatchAsync(new CreateJourneyQuery(userId, date, carId, start, end, fromPlaceId, toPlaceIds, rateId))
					.IfSomeAsync(x => journeyIds.Add(x));

				date = date.AddDays(Rnd.NumberF.GetInt32(0, 3) * -1);
			}

			return journeyIds;
		}
	}
}
