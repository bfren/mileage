// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data.Attributes;
using Jeebs.Data.Map;
using Mileage.Persistence.Common;

namespace Mileage.Persistence.Tables;

/// <summary>
/// Place Table definition
/// </summary>
public sealed record class PlaceTable() : Table(Constants.Schema, TableName)
{
	/// <summary>
	/// Table name - used as a prefix for each column
	/// </summary>
	public static readonly string TableName = "place";

	/// <summary>
	/// Place ID
	/// </summary>
	[Id]
	public string Id =>
		"place_id";

	/// <summary>
	/// Place Version
	/// </summary>
	[Version]
	public string Version =>
		"place_version";

	/// <summary>
	/// Place User ID
	/// </summary>
	public string UserId =>
		"place_user_id";

	/// <summary>
	/// Place Description
	/// </summary>
	public string Description =>
		"place_description";

	/// <summary>
	/// Place Postcode
	/// </summary>
	public string Postcode =>
		"place_postcode";
}
