// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

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

	public string Id =>
		TableName + nameof(Id);

	public string Version =>
		TableName + nameof(Version);

	public string UserId =>
		TableName + nameof(UserId);

	public string Description =>
		TableName + nameof(Description);
}
