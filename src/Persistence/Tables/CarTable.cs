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
	public static readonly string TableName = "Car";

	/// <summary>
	/// Car ID
	/// </summary>
	[Id]
	public string Id =>
		TableName + nameof(Id);

	/// <summary>
	/// Car Version
	/// </summary>
	[Version]
	public string Version =>
		TableName + nameof(Version);

	/// <summary>
	/// Car User ID
	/// </summary>
	public string UserId =>
		TableName + nameof(UserId);

	/// <summary>
	/// Car Description
	/// </summary>
	public string Description =>
		TableName + nameof(Description);
}
