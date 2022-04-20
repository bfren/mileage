// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data.Attributes;
using Jeebs.Data.Map;
using Mileage.Persistence.Common;

namespace Mileage.Persistence.Tables;

/// <summary>
/// Car Table definition
/// </summary>
public sealed record class CarTable() : Table(Constants.Schema, TableName)
{
	/// <summary>
	/// Table name - used as a prefix for each column
	/// </summary>
	public static readonly string TableName = "car";

	/// <summary>
	/// Car ID
	/// </summary>
	[Id]
	public string Id =>
		"car_id";

	/// <summary>
	/// Car Version
	/// </summary>
	[Version]
	public string Version =>
		"car_version";

	/// <summary>
	/// Car User ID
	/// </summary>
	public string UserId =>
		"car_user_id";

	/// <summary>
	/// Car Description
	/// </summary>
	public string Description =>
		"car_description";

	/// <summary>
	/// Car Number Plate
	/// </summary>
	public string NumberPlate =>
		"car_number_plate";
}
