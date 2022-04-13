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
	public static readonly string TableName = "Place";

	/// <summary>
	/// Place ID
	/// </summary>
	[Id]
	public string Id =>
		TableName + nameof(Id);

	/// <summary>
	/// Place Version
	/// </summary>
	[Version]
	public string Version =>
		TableName + nameof(Version);

	/// <summary>
	/// Place User ID
	/// </summary>
	public string UserId =>
		TableName + nameof(UserId);

	/// <summary>
	/// Place Description
	/// </summary>
	public string Description =>
		TableName + nameof(Description);

	/// <summary>
	/// Place Postcode
	/// </summary>
	public string Postcode =>
		TableName + nameof(Postcode);
}
