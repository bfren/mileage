// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data.Attributes;
using Jeebs.Data.Map;
using Mileage.Persistence.Common;

namespace Mileage.Persistence.Tables;

/// <summary>
/// Journey Table definition
/// </summary>
public sealed record class JourneyTable() : Table(Constants.Schema, TableName)
{
	/// <summary>
	/// Table name - used as a prefix for each column
	/// </summary>
	public static readonly string TableName = "Journey";

	/// <summary>
	/// Journey ID
	/// </summary>
	[Id]
	public string Id =>
		TableName + nameof(Id);

	/// <summary>
	/// Journey Version
	/// </summary>
	[Version]
	public string Version =>
		TableName + nameof(Version);

	/// <summary>
	/// Journey User ID
	/// </summary>
	public string UserId =>
		TableName + nameof(UserId);

	/// <summary>
	/// Journey Date
	/// </summary>
	public string Date =>
		TableName + nameof(Date);

	/// <summary>
	/// Journey Car ID
	/// </summary>
	public string CarId =>
		TableName + nameof(CarId);

	/// <summary>
	/// Journey Start Miles
	/// </summary>
	public string StartMiles =>
		TableName + nameof(StartMiles);

	/// <summary>
	/// Journey End Miles
	/// </summary>
	public string EndMiles =>
		TableName + nameof(EndMiles);

	/// <summary>
	/// Journey From Place ID
	/// </summary>
	public string FromPlaceId =>
		TableName + nameof(FromPlaceId);

	/// <summary>
	/// Journey To Place IDs
	/// </summary>
	public string ToPlaceIds =>
		TableName + nameof(ToPlaceIds);

	/// <summary>
	/// Journey Rate ID
	/// </summary>
	public string RateId =>
		TableName + nameof(RateId);
}
