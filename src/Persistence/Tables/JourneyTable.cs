// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

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

	public string Id =>
		TableName + nameof(Id);

	public string Version =>
		TableName + nameof(Version);

	public string UserId =>
		TableName + nameof(UserId);

	public string Date =>
		TableName + nameof(Date);

	public string CarId =>
		TableName + nameof(CarId);

	public string StartMiles =>
		TableName + nameof(StartMiles);

	public string EndMiles =>
		TableName + nameof(EndMiles);

	public string FromPlaceId =>
		TableName + nameof(FromPlaceId);

	public string ToPlaceIds =>
		TableName + nameof(ToPlaceIds);

	public string RateId =>
		TableName + nameof(RateId);
}
