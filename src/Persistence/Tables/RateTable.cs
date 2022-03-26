// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data.Map;
using Mileage.Persistence.Common;

namespace Mileage.Persistence.Tables;

/// <summary>
/// Rate Table definition
/// </summary>
public sealed record class RateTable() : Table(Constants.Schema, TableName)
{
	/// <summary>
	/// Table name - used as a prefix for each column
	/// </summary>
	public static readonly string TableName = "Rate";

	/// <summary>
	/// Rate ID
	/// </summary>
	public string Id =>
		TableName + nameof(Id);

	/// <summary>
	/// Rate Version
	/// </summary>
	public string Version =>
		TableName + nameof(Version);

	/// <summary>
	/// Rate User ID
	/// </summary>
	public string UserId =>
		TableName + nameof(UserId);

	/// <summary>
	/// Rate Amount per Mile (in GBP)
	/// </summary>
	public string AmountPerMileGBP =>
		TableName + nameof(AmountPerMileGBP);
}
