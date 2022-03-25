// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

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

	public string Id =>
		TableName + nameof(Id);

	public string Version =>
		TableName + nameof(Version);

	public string UserId =>
		TableName + nameof(UserId);

	public string DefaultCarId =>
		TableName + nameof(DefaultCarId);

	public string DefaultFromPlaceId =>
		TableName + nameof(DefaultFromPlaceId);
}
