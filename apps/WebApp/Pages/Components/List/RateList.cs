// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Globalization;
using Mileage.Domain.GetRates;
using Mileage.Persistence.Common.StrongIds;

namespace Mileage.WebApp.Pages.Components.List;

public sealed class RateListViewComponent : ListSingleViewComponent<GetRatesModel, RateId>
{
	public RateListViewComponent() : base("rate", x => x.AmountPerMileGBP.ToString("0.00", CultureInfo.InvariantCulture)) { }
}
