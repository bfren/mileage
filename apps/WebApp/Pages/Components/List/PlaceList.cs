// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Domain.GetPlacesForUser;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.List;

public sealed class PlaceListViewComponent : ListViewComponent<GetPlacesForUserModel, PlaceId>
{
	public PlaceListViewComponent() : base("place", x => x.Description) { }
}
