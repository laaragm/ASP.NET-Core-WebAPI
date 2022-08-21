using System;
using AspNetCore_WebApi_DevIO.Extensions;
using Elmah.Io.AspNetCore;
using Elmah.Io.AspNetCore.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore_WebApi_DevIO.Configuration
{
	public static class LoggerConfig
	{
		public static IServiceCollection AddLoggingConfig(this IServiceCollection services, IConfiguration configuration)
		{
			//services.AddElmahIo(options =>
			//{
			//	options.ApiKey = "09242430e83c4171bbe0358709b402ed";
			//	options.LogId = new Guid("3f71f7e7-93bd-4485-815e-e5762d548826");
			//});

			services.AddHealthChecks()
				// Integration with elmah
				//.AddElmahIoPublisher(options =>
				//{
				//	options.ApiKey = "09242430e83c4171bbe0358709b402ed";
				//	options.LogId = new Guid("3f71f7e7-93bd-4485-815e-e5762d548826");
				//	options.HeartbeatId = "ASP.NET Core WebAPI";
				//})
				.AddCheck("Products", new SqlServerHealthCheck(configuration.GetConnectionString("DatabaseConnectionString"))) // Products check scenario
				.AddSqlServer(configuration.GetConnectionString("DatabaseConnectionString"), name: "SQLDatabase"); // Check if we're able to connect with the DB

			services.AddHealthChecksUI().AddSqlServerStorage(configuration.GetConnectionString("DatabaseConnectionString"));

			return services;
		}

		public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
		{
			app.UseElmahIo();

			return app;
		}
	}
}
