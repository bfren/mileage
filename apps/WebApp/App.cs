// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Apps.Web;
using Jeebs.Mvc.Auth;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain;
using Mileage.Persistence.Common.Ids;
using Serilog;
using Wrap.Caching;
using Wrap.Mvc;

namespace Mileage.WebApp;

public sealed class App : RazorApp
{
	public override void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
	{
		base.ConfigureServices(ctx, services);

		_ = services.AddData();
		_ = services.AddAuthentication(ctx.Configuration);

		_ = services
			.AddMemoryCache()
			.AddWrapCache<CarId>()
			.AddWrapCache<PlaceId>()
			.AddWrapCache<RateId>();
	}

	protected override void ConfigureServicesMvcOptions(HostBuilderContext ctx, MvcOptions opt)
	{
		base.ConfigureServicesMvcOptions(ctx, opt);
		opt.ModelBinderProviders.AddWrapModelBinders();
	}

	protected override void ConfigureAuth(WebApplication app, IConfiguration config)
	{
		_ = app.UseAuthentication();
		base.ConfigureAuth(app, config);
	}

	public override void ConfigureSerilog(HostBuilderContext ctx, LoggerConfiguration loggerConfig)
	{
		base.ConfigureSerilog(ctx, loggerConfig);
		_ = loggerConfig.MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning);
	}
}
