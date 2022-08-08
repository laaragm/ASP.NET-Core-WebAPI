using AspNetCore_WebApi_DevIO.Configuration;
using AspNetCore_WebAPI_DevIO.Data.Context;
using AutoMapper;
using DevIO.Api.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore_WebApi_DevIO
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<DatabaseContext>(options =>
			{
				options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnectionString"));
			});

			services.AddIdentityConfig(Configuration);

			services.AddAutoMapper(typeof(Startup)); // When we add the 'typeof(Startup' it basically says that it'll resolve everything from this particular assembly

			services.AddApiConfig();

			services.AddControllers();

			services.ResolveDependencies();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseApiConfig(env);
		}
	}
}
