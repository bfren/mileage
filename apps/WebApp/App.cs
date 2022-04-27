// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Apps.Web;
using Jeebs.Auth.Data.Clients.PostgreSql;
using Jeebs.Mvc.Auth;
using MaybeF.Caching;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain;
using Mileage.Persistence.Common.StrongIds;
using Serilog;
using StrongId.Mvc;

namespace Mileage.WebApp;

public sealed class App : RazorApp
{
	public override void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
	{
		base.ConfigureServices(ctx, services);

		_ = services.AddData();

		_ = services.AddAuthentication(ctx.Configuration)
			.WithData<PostgreSqlDbClient>(true)
			.WithJwt();

		_ = services
			.AddMemoryCache()
			.AddMaybeCache<CarId>()
			.AddMaybeCache<PlaceId>()
			.AddMaybeCache<RateId>();
	}

	protected override void ConfigureServicesMvcOptions(MvcOptions opt)
	{
		base.ConfigureServicesMvcOptions(opt);
		opt.AddStrongIdModelBinder();
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
