// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Auth.Data;
using Jeebs.Auth.Data.Clients.PostgreSql;
using Jeebs.Cqrs;
using Jeebs.Data;
using Microsoft.Extensions.DependencyInjection;
using Mileage.Persistence;
using Mileage.Persistence.Clients.PostgreSql;
using Mileage.Persistence.Repositories;

namespace Mileage.Queries;

/// <summary>
/// <see cref="IServiceCollection"/> extensions
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Add data classes to the DI container
	/// </summary>
	/// <param name="services"></param>
	public static IServiceCollection AddData(this IServiceCollection services)
	{
		// Add database
		_ = services.AddSingleton<IDb, MileageDb>();
		_ = services.AddAuthData<PostgreSqlDbClient>(true);
		_ = services.AddTransient<Migrator, PostgreSqlMigrator>();

		// Add repositories
		_ = services.AddTransient<ICarRepository, CarRepository>();
		_ = services.AddTransient<ISettingsRepository, SettingsRepository>();
		_ = services.AddTransient<IJourneyRepository, JourneyRepository>();
		_ = services.AddTransient<IPlaceRepository, PlaceRepository>();
		_ = services.AddTransient<IRateRepository, RateRepository>();

		// Add CQRS
		_ = services.AddCqrs();

		// Return
		return services;
	}
}
