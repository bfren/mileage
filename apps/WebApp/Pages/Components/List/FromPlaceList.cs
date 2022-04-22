// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Domain.GetPlaces;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.List;

public sealed class FromPlaceListViewComponent : ListSingleViewComponent<GetPlacesModel, PlaceId>
{
	public FromPlaceListViewComponent() : base("place", x => x.Description) { }
}
