// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data.Attributes;
using Jeebs.Data.Map;
using Mileage.Persistence.Common;

namespace Mileage.Persistence.Tables;

/// <summary>
/// Settings Table definition
/// </summary>
public sealed record class SettingsTable() : Table(Constants.Schema, TableName)
{
	/// <summary>
	/// Table name - used as a prefix for each column
	/// </summary>
	public static readonly string TableName = "settings";

	/// <summary>
	/// Settings ID
	/// </summary>
	[Id]
	public string Id =>
		"settings_id";

	/// <summary>
	/// Settings Version
	/// </summary>
	[Version]
	public string Version =>
		"settings_version";

	/// <summary>
	/// Settings User ID
	/// </summary>
	public string UserId =>
		"settings_user_id";

	/// <summary>
	/// Settings Default Car ID
	/// </summary>
	public string DefaultCarId =>
		"settings_default_car_id";

	/// <summary>
	/// Settings Default From Place ID
	/// </summary>
	public string DefaultFromPlaceId =>
		"settings_default_from_place_id";

	/// <summary>
	/// Settings Default Rate ID
	/// </summary>
	public string DefaultRateId =>
		"settings_default_rate_id";
}
