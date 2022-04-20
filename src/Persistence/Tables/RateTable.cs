// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data.Attributes;
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
	public static readonly string TableName = "rate";

	/// <summary>
	/// Rate ID
	/// </summary>
	[Id]
	public string Id =>
		"rate_id";

	/// <summary>
	/// Rate Version
	/// </summary>
	[Version]
	public string Version =>
		"rate_version";

	/// <summary>
	/// Rate User ID
	/// </summary>
	public string UserId =>
		"rate_user_id";

	/// <summary>
	/// Rate Amount per Mile (in GBP)
	/// </summary>
	public string AmountPerMileGBP =>
		"rate_amount";
}
