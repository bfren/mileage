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
	public static readonly string TableName = "journey";

	/// <summary>
	/// Journey ID
	/// </summary>
	[Id]
	public string Id =>
		"journey_id";

	/// <summary>
	/// Journey Version
	/// </summary>
	[Version]
	public string Version =>
		"journey_version";

	/// <summary>
	/// Journey User ID
	/// </summary>
	public string UserId =>
		"journey_user_id";

	/// <summary>
	/// Journey Date
	/// </summary>
	public string Day =>
		"journey_date";

	/// <summary>
	/// Journey Car ID
	/// </summary>
	public string CarId =>
		"journey_car_id";

	/// <summary>
	/// Journey Start Miles
	/// </summary>
	public string StartMiles =>
		"journey_start_miles";

	/// <summary>
	/// Journey End Miles
	/// </summary>
	public string EndMiles =>
		"journey_end_miles";

	/// <summary>
	/// Journey From Place ID
	/// </summary>
	public string FromPlaceId =>
		"journey_from_place_id";

	/// <summary>
	/// Journey To Place IDs
	/// </summary>
	public string ToPlaceIds =>
		"journey_to_place_ids";

	/// <summary>
	/// Journey Rate ID
	/// </summary>
	public string RateId =>
		"journey_rate_id";
}
