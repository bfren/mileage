// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Domain.GetJourneys;

namespace Mileage.WebApp.Pages.Journeys;

public sealed record class ListModel
{
	public IEnumerable<JourneyModel> Journeys { get; init; }

	public string DeleteHandler { get; init; }

	public string ReplaceId { get; init; }

	public ListModel(string deleteHandler) : this(deleteHandler, string.Empty) { }

	public ListModel(string deleteHandler, string replaceId) =>
		(Journeys, DeleteHandler, ReplaceId) = (new List<JourneyModel>(), deleteHandler, replaceId);
}
