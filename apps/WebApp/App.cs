// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Apps.Web;
using Jeebs.Auth.Data.Clients.PostgreSql;
using Jeebs.Mvc.Auth;
using Jeebs.Mvc.Data;
using Microsoft.AspNetCore.Mvc;
using Mileage.Domain;

namespace Mileage.WebApp;

public sealed class App : RazorApp
{
	public override void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
	{
		base.ConfigureServices(ctx, services);

		_ = services.AddData();

		_ = services.AddAuthentication(ctx.Configuration)
			.WithData<PostgreSqlDbClient>(false)
			.WithJwt();
	}

	protected override void ConfigureServicesMvcOptions(MvcOptions opt)
	{
		base.ConfigureServicesMvcOptions(opt);

		opt.AddStrongIdModelBinding();
	}

	protected override void ConfigureAuth(WebApplication app, IConfiguration config)
	{
		app.UseAuthentication();
		base.ConfigureAuth(app, config);
	}
}
