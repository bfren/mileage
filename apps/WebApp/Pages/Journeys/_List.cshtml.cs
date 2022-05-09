// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Domain.GetJourneys;

namespace Mileage.WebApp.Pages.Journeys;

public sealed record class ListModel
{
	public IEnumerable<JourneyModel> Journeys { get; init; } = new List<JourneyModel>();

	public string DeleteHandler { get; init; }

	public ListModel(string deleteHandler) =>
		DeleteHandler = deleteHandler;
}
