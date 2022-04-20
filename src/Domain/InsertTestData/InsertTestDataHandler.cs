// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

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
				from r0 in Dispatcher.DispatchAsync(new CreateRateQuery(u0, Rnd.NumberF.GetSingle(min: 0.1f, max: 0.9f)))
				from r1 in Dispatcher.DispatchAsync(new CreateRateQuery(u0, Rnd.NumberF.GetSingle(min: 0.1f, max: 0.9f)))
				from j0 in insertJourneys(u0, c0, p0, new[] { p1, p2 }, r0)
				from j1 in insertJourneys(u0, c1, p0, new[] { p1, p2 }, r1)
				from s0 in Dispatcher.DispatchAsync(new SaveSettingsCommand(u0, new(0, c0, null)))
				select true;

		return await q.AuditAsync(
			none: r => Log.Msg(r, LogLevel.Error)
		);

		async Task<Maybe<IEnumerable<JourneyId>>> insertJourneys(AuthUserId userId, CarId carId, PlaceId fromPlaceId, PlaceId[] toPlaceIds, RateId rateId)
		{
			Log.Inf("Inserting test journeys.");
			var mileage = new List<(uint start, uint end)>();
			const int number = 10;

			// Generate random mileage numbers
			for (var i = 0; i < number; i++)
			{
				var start = Rnd.UInt;
				var end = start + Rnd.UInt;
				mileage.Add((start, end));
			}
			mileage.Sort((a, b) => a.start.CompareTo(b.start));

			// Create each journey
			var journeyIds = new List<JourneyId>();
			for (var i = 0; i < number; i++)
			{
				var (start, end) = mileage[i];
				_ = await Dispatcher.DispatchAsync(
					new CreateJourneyQuery(userId, Rnd.Date, carId, start, (Rnd.Flip || Rnd.Flip) ? end : null, fromPlaceId, toPlaceIds, rateId)
				).IfSomeAsync(
					x => journeyIds.Add(x)
				);
			}

			return journeyIds;
		}
	}
}
