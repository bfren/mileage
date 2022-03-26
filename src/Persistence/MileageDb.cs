// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Collections;
using Jeebs.Config.Db;
using Jeebs.Data;
using Jeebs.Logging;
using Microsoft.Extensions.Options;
using Mileage.Persistence.Common.StrongIds;
using Mileage.Persistence.Entities;
using Mileage.Persistence.Tables;

namespace Mileage.Persistence;

/// <summary>
/// Mileage Database
/// </summary>
public sealed class MileageDb : Db
{
	/// <summary>
	/// Car table
	/// </summary>
	public CarTable Car { get; init; } = new();

	/// <summary>
	/// Config table
	/// </summary>
	public SettingsTable Configuration { get; init; } = new();

	/// <summary>
	/// Journey table
	/// </summary>
	public JourneyTable Journey { get; init; } = new();

	/// <summary>
	/// Place table
	/// </summary>
	public PlaceTable Place { get; init; } = new();

	/// <summary>
	/// Rate table
	/// </summary>
	public RateTable Rate { get; init; } = new();

	/// <summary>
	/// Inject dependencies and use default database connection
	/// </summary>
	/// <param name="client"></param>
	/// <param name="config"></param>
	/// <param name="log"></param>
	public MileageDb(IDbClient client, IOptions<DbConfig> config, ILog<MileageDb> log) :
		base(client, config.Value.GetConnection(), log)
	{
		// Map entities
		_ = Map<CarEntity>.To(Car);
		_ = Map<SettingsEntity>.To(Configuration);
		_ = Map<JourneyEntity>.To(Journey);
		_ = Map<PlaceEntity>.To(Place);
		_ = Map<RateEntity>.To(Rate);

		// Add type handlers
		TypeMap.AddStrongIdTypeHandlers();
		TypeMap.AddJsonTypeHandler<ImmutableList<PlaceId>>();
	}
}
