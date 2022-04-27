// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Domain.GetPlaces;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.List;

public sealed class ToPlacesListViewComponent : ListMultipleViewComponent<GetPlacesModel, PlaceId>
{
	public ToPlacesListViewComponent() : base("place", x => x.Description,
		x => x.Description + (x.Postcode is string postcode ? $" ({postcode})" : string.Empty)
	)
	{ }
}
