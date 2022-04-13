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
	public static readonly string TableName = "Settings";

	/// <summary>
	/// Settings ID
	/// </summary>
	[Id]
	public string Id =>
		TableName + nameof(Id);

	/// <summary>
	/// Settings Version
	/// </summary>
	[Version]
	public string Version =>
		TableName + nameof(Version);

	/// <summary>
	/// Settings User ID
	/// </summary>
	public string UserId =>
		TableName + nameof(UserId);

	/// <summary>
	/// Settings Default Car ID
	/// </summary>
	public string DefaultCarId =>
		TableName + nameof(DefaultCarId);

	/// <summary>
	/// Settings Default Place ID
	/// </summary>
	public string DefaultFromPlaceId =>
		TableName + nameof(DefaultFromPlaceId);
}
